namespace Festivo.Shared.Services;

public interface IQueueService
{
    Task WriteToQueue(string exchangeName, string routingKey, string message);
}