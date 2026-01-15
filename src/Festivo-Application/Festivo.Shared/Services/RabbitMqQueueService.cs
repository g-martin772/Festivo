using System.Text;
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
    
    public async Task WriteToQueue(string exchangeName, string routingKey, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        if (_channel != null)
            await _channel.BasicPublishAsync(
                exchange: exchangeName,
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
}