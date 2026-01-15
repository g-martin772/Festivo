using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Festivo.LoggingService.Services;

public class ConsumeErrorMessagesBgService(
    IConnectionFactory connectionFactory,
    ILogger<ConsumeErrorMessagesBgService> logger) : BackgroundService
{
    private IChannel? _channel;
    
    private const int MaxRetries = 5;
    private const int RetryDelayMs = 2000;
    
    private const string QueueName = "dead-letter-queue";
    private const string ExchangeName = "message-exchange";
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConnectAsync(factory: connectionFactory);

        if (_channel == null)
            return;
        
        var queue = await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: "#.error",
            cancellationToken: stoppingToken);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = System.Text.Encoding.UTF8.GetString(body);
                var encodedJson = JsonSerializer.Deserialize<string>(message);
                logger.LogInformation("Received message: {Message}", encodedJson);
                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                await _channel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);
            }   
        };
        
        await _channel.BasicConsumeAsync(
            queue: queue.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
        
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
    
    private async Task ConnectAsync(IConnectionFactory factory)
    {
        try
        {
            for (var i = 0; i < MaxRetries; i++)
            {
                try
                {
                    var connection = await factory.CreateConnectionAsync();
                    _channel = await connection.CreateChannelAsync();
                    logger.LogInformation("Connected to RabbitMQ broker.");
                    break;
                }
                catch(BrokerUnreachableException)
                {
                    logger.LogWarning("RabbitMQ broker unreachable, retrying... " +
                                       "({Retry}/{MaxRetries})", i + 1, MaxRetries);
                    await Task.Delay(RetryDelayMs);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError("Failed to connect to RabbitMQ: {EMessage}", e.Message);
        }
    }
}