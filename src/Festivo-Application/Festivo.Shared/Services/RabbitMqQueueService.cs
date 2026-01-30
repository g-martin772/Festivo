using System.Text;
using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Festivo.Shared.Services;

public class RabbitMqQueueService : IQueueService
{
    private IChannel? _channel;
    private readonly ILogger _logger;
    
    private const int MaxRetries = 5;
    private const int RetryDelayMs = 2000;
    
    public RabbitMqQueueService(
        IConnectionFactory factory, 
        ILogger<RabbitMqQueueService> logger)
    {
        _logger = logger;
        ConnectAsync(factory);
    }
    
    public async Task WriteToQueue(
        string routingKey, string message, 
        string serviceName, string eventName)
    {
        var evt = new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Source = new Uri($"service://{serviceName}"),
            Time = DateTimeOffset.UtcNow,
            Type = eventName,
            DataContentType = "application/json",
            Data = new
            {
                message
            }
        };
        
        var body = Encoding.UTF8.GetBytes(evt.Data.ToString() ?? string.Empty);
        if (_channel != null)
            await _channel.BasicPublishAsync(
                exchange: "messages",
                routingKey: routingKey,
                body: body
            );
        else
        {
            _logger.LogError("Failed to publish message to RabbitMQ: {EMessage}", message);
        }
    }

    private async void ConnectAsync(IConnectionFactory factory)
    {
        try
        {
            for (var i = 0; i < MaxRetries; i++)
            {
                try
                {
                    var connection = await factory.CreateConnectionAsync();
                    _channel = await connection.CreateChannelAsync();
                    _logger.LogInformation("Connected to RabbitMQ broker.");
                    break;
                }
                catch(BrokerUnreachableException)
                {
                    _logger.LogWarning("RabbitMQ broker unreachable, retrying... " +
                                       "({Retry}/{MaxRetries})", i + 1, MaxRetries);
                    await Task.Delay(RetryDelayMs);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to connect to RabbitMQ: {EMessage}", e.Message);
        }
    }
    
    private async void DeclareBasicExchange()
    {
        try
        {
            if (_channel != null)
            {
                await _channel.ExchangeDeclareAsync(
                    exchange: "messages",
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false
                );
                _logger.LogInformation("Declared basic exchange 'messages'.");
            }
            else
            {
                _logger.LogError("Cannot declare exchange: RabbitMQ channel is not established.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to declare exchange: {EMessage}", e.Message);
        }
    }
}