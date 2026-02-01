using System.Text;
using System.Text.Json;
using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Festivo.Shared.Helper;

public static class RabbitMqHelper
{
    public static async Task DeclareExchange(
        IChannel channel, 
        string exchangeName, 
        string exchangeType, 
        bool durable = true,
        bool autoDelete = false)
    {
        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: exchangeType,
            durable: durable,
            autoDelete: autoDelete
        );
    }
    
    public static async Task DeclareQueue(
        IChannel channel, 
        string queueName, 
        bool durable = true, 
        bool autoDelete = false, 
        bool exclusive = false,
        Dictionary<string, object?> arguments = null,
        CancellationToken cancellationToken = default)
    {
        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: durable,
            autoDelete: autoDelete,
            exclusive: exclusive,
            arguments: arguments!,
            cancellationToken: cancellationToken
        );
    }
    
    public static async Task BindQueue(
        IChannel channel, 
        string queueName, 
        string exchangeName, 
        string routingKey = "",
        CancellationToken cancellationToken = default)
    {
        await channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName,
            routingKey: routingKey,
            cancellationToken: cancellationToken
        );
    }

    public static async Task AddConsumer(
        ILogger logger, 
        IChannel channel, 
        string queueName, 
        bool autoAck = false, 
        CancellationToken cancellationToken = default)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var encodedMessage = JsonSerializer.Deserialize<CloudEvent>(message);
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var data = encodedMessage?.Data;
                
                logger.LogInformation("[{Timestamp}] {QueueName}: \"{Data}\"", timestamp, queueName, data);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false,
                    cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false,
                    cancellationToken: cancellationToken);
            }
        };
        
        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: autoAck,
            consumer: consumer, cancellationToken: cancellationToken
        );
    }
    
    public static async Task WriteToQueue(
        IChannel channel, ILogger logger,
        string routingKey, object message, 
        string serviceName, string eventName,
        CancellationToken cancellationToken = default)
    {
        var evt = new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Source = new Uri($"service://{serviceName}"),
            Time = DateTimeOffset.UtcNow,
            Type = eventName,
            DataContentType = "application/json",
            Data = JsonSerializer.Serialize(message)
        };
        
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        logger.LogInformation("[{Timestamp}] {EventName}: \"{Message}\"", timestamp, eventName, message);
        
        var bodyData = JsonSerializer.Serialize(evt);
        var body = Encoding.UTF8.GetBytes(bodyData);
        await channel.BasicPublishAsync(
            exchange: "messages",
            routingKey: routingKey,
            body: body, 
            cancellationToken: cancellationToken
        );
    }
}