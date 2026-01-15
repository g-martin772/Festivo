using System.Text;
using System.Text.Json;
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
        ConnectionFactory factory, 
        ILogger<RabbitMqQueueService> logger)
    {
        _logger = logger;
        ConnectAsync(factory);
    }


    public string ReadFromQueue(string queueName)
    {
        throw new NotImplementedException();
    }

    public void WriteToQueue(string queueName, string message)
    {
        throw new NotImplementedException();
    }

    private async void ConnectAsync(ConnectionFactory factory)
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
                    await SendTestMessage();
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

    private async Task SendTestMessage()
    {
        await _channel?.QueueDeclareAsync(
            queue: "test", 
            durable: false, 
            exclusive: false, 
            autoDelete: false,
            arguments: null
        )!;

        var data = JsonSerializer.Serialize(new
        {
            purpose = "Test: RabbitMQ Connection",
            message = "Connected to RabbitMQ successfully."
        });
        
        var body = Encoding.UTF8.GetBytes(data);
        await _channel.BasicPublishAsync(
            exchange: string.Empty, 
            routingKey: "test", 
            body: body
        );
    }
}