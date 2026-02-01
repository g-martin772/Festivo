using System.Text.Json;
using Festivo.Shared.Helper;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Festivo.CallbackService.Services;

public class QueueBackgroundService(
    IConnectionFactory connectionFactory, 
    ILogger<QueueBackgroundService> logger) : BackgroundService
{
    private IChannel? _channel;
    
    private const int MaxRetries = 5;
    private const int RetryDelayMs = 2000;
    
    private const string ExchangeName = "messages";
    private static readonly List<string> Queues = [];
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConnectAsync(factory: connectionFactory);

        if (_channel == null)
            return;
        
        await RabbitMqHelper.DeclareExchange(channel: _channel, exchangeName: ExchangeName, ExchangeType.Topic);

        foreach (var name in Queues)
        {
            await RabbitMqHelper.DeclareQueue(
                channel: _channel, 
                queueName: name, 
                arguments: new Dictionary<string, object?>()
                {
                    { "x-dead-letter-exchange", ExchangeName },
                    { "x-dead-letter-routing-key", "3-callback.error" }
                },
                cancellationToken: stoppingToken
            );
        }

        foreach (var name in Queues)
        {
            await RabbitMqHelper.BindQueue(
                channel: _channel,
                queueName: name,
                exchangeName: "messages",
                routingKey: name,
                cancellationToken: stoppingToken
            );
        }

        foreach (var name in Queues)
        {
            await RabbitMqHelper.AddConsumer(
                channel: _channel,
                logger: logger,
                queueName: name,
                cancellationToken: stoppingToken
            );
        }
        
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