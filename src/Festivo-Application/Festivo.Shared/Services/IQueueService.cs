namespace Festivo.Shared.Services;

public interface IQueueService
{
    string ReadFromQueue(string queueName);
    void WriteToQueue(string queueName, string message);
}