using System.Net.Mime;
using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Festivo.Shared.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Festivo.Shared.Services;

public class EventBus(
    ILogger<EventBus> logger,
    IConnectionFactory connectionFactory,
    IHostEnvironment env,
    QueueBindingCollection queueBindings) : IHostedService
{
    public const string MainExchangeName = "festivo";
    public const string DeadLetterExchangeName = "festivo.dl";

    public delegate Task EventHandler<T>(CloudEvent e, T body, BasicDeliverEventArgs args,
        CancellationToken ct);
    
    public TaskCompletionSource Initialization { get; } = new();

    IConnection? m_Connection;
    DefaultObjectPool<IChannel>? m_ChannelPool;

    public async Task StartAsync(CancellationToken ct)
    {
        m_Connection = await connectionFactory.CreateConnectionAsync(ct);
        var poolPolicy = new ChannelPooledObjectPolicy(m_Connection);
        m_ChannelPool = new DefaultObjectPool<IChannel>(poolPolicy);

        var channel = GetChannel();
        await channel.ExchangeDeclareAsync(DeadLetterExchangeName, ExchangeType.Topic, durable: true, autoDelete: false,
            cancellationToken: ct);
        await channel.ExchangeDeclareAsync(MainExchangeName, ExchangeType.Topic, durable: true, autoDelete: false,
            cancellationToken: ct);

        foreach (var binding in queueBindings) await DeclareQueue(binding.QueueName, binding.RoutingKey, ct);
        
        Initialization.SetResult();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (m_Connection is not null)
            await m_Connection.DisposeAsync();
    }

    public IChannel GetChannel() =>
        m_ChannelPool is null ? throw new InvalidOperationException("No channel pool available.") : m_ChannelPool.Get();

    public async Task DeclareQueue(string queueName, string routingKey, CancellationToken ct = default)
    {
        var channel = GetChannel();
        var options = new Dictionary<string, object?>()
        {
            { "x-dead-letter-exchange", DeadLetterExchangeName },
            { "x-dead-letter-routing-key", $"com.festivo.dlq.{queueName}" }
        };
        await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, options,
            cancellationToken: ct);
        await channel.QueueBindAsync(queueName, MainExchangeName, routingKey, cancellationToken: ct);
    }

    public async Task AddConsumerAsync<T>(
        string queueName,
        EventHandler<T> handler,
        CancellationToken cancellationToken = default)
    {
        var channel = GetChannel();
        var formatter = new JsonEventFormatter();
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var encodedMessage =
                    formatter.DecodeStructuredModeMessage(body, contentType: new ContentType("application/json"),
                        extensionAttributes: null);
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var data = encodedMessage.Data;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var messageBody = JsonSerializer.Deserialize<T>(
                                      ((JsonElement)(data ?? throw new NullReferenceException())).GetString()!, options)
                                  ?? throw new InvalidOperationException("Failed to deserialize event data.");

                logger.LogInformation("[{Timestamp}] {QueueName}: \"{Data}\"", timestamp, queueName, data);

                await handler(encodedMessage, messageBody, eventArgs, cancellationToken);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false,
                    cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to process message: {Message}", e.Message);
                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false,
                    cancellationToken: cancellationToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken
        );
    }

    public async Task PublishMessageAsync<T>(T message, CancellationToken ct = default)
    {
        var channel = GetChannel();
        var eventName = EventTypeRegistry.GetEventTypeName(typeof(T));

        var evt = new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Source = new Uri($"service://{env.ApplicationName}"),
            Time = DateTimeOffset.UtcNow,
            Type = eventName,
            DataContentType = "application/json",
            Data = JsonSerializer.Serialize(message)
        };

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        logger.LogInformation("[{Timestamp}] {EventName}: \"{Message}\"", timestamp, eventName, message);

        var formatter = new JsonEventFormatter();
        var bodyData = formatter.EncodeStructuredModeMessage(evt, out _);

        await channel.BasicPublishAsync(
            exchange: MainExchangeName,
            routingKey: eventName,
            body: bodyData.ToArray(),
            cancellationToken: ct
        );
    }
}