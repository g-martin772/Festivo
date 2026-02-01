namespace Festivo.Shared.Events;

public class NotificationSentEvent
{
    public string NotificationId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

public class BroadcastNotificationSentEvent
{
    public string NotificationId { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int RecipientCount { get; set; }
    public DateTime SentAt { get; set; }
}

