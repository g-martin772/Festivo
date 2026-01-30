namespace Festivo.Shared.Services;

public interface IQueueService
{
    Task WriteToQueue(string routingKey, string message, string serviceName, string eventName);
}