using System.Text.Json;
using Festivo.Shared.Helper;
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
    private const string ExchangeName = "messages";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConnectAsync(factory: connectionFactory);

        if (_channel == null)
            return;

        await RabbitMqHelper.DeclareQueue(
            queueName: QueueName,
            channel: _channel,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken
        );

        await RabbitMqHelper.BindQueue(
            queueName: QueueName,
            channel: _channel,
            exchangeName: ExchangeName,
            routingKey: "#.error",
            cancellationToken: stoppingToken
        );

        await RabbitMqHelper.AddConsumer(
            queueName: QueueName,
            channel: _channel,
            logger: logger,
            cancellationToken: stoppingToken
        );
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