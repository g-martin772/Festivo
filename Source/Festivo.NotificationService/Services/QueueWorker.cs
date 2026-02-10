using CloudNative.CloudEvents;
using Festivo.NotificationService.Hubs;
using Festivo.Shared.Events;
using Festivo.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;

namespace Festivo.NotificationService.Services;

public partial class QueueWorker(
    EventBus eventBus,
    IHubContext<NotificationHub> hubContext,
    ILogger<QueueWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await eventBus.Initialization.Task;

        await eventBus.AddConsumerAsync<TicketPurchasedEvent>("NotificationForTicketPurchased", HandleTicketPurchased, stoppingToken);
        await eventBus.AddConsumerAsync<EntryGrantedEvent>("NotificationForEntryGranted", HandleEntryGranted, stoppingToken);
        await eventBus.AddConsumerAsync<EntryDeniedEvent>("NotificationForEntryDenied", HandleEntryDenied, stoppingToken);
        await eventBus.AddConsumerAsync<OccupancyUpdatedEvent>("NotificationForOccupancyUpdated", HandleOccupancyUpdated,
            stoppingToken);
        await eventBus.AddConsumerAsync<CapacityWarningIssuedEvent>("NotificationForCapacityWarning", HandleCapacityWarning,
            stoppingToken);
    }

    private async Task HandleTicketPurchased(CloudEvent @event, TicketPurchasedEvent body, BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        LogTicketPurchased(logger, body.TicketCode,
            body.CustomerId);

        var notification = new
        {
            Type = "TicketPurchased",
            TicketCode = body.TicketCode.ToString(),
            TicketType = body.TicketType,
            Price = body.Price,
            PurchaseDate = body.PurchaseDate,
            CustomerId = body.CustomerId.ToString(),
            EventId = body.EventId.ToString(),
            Message = $"Ticket purchased successfully! Type: {body.TicketType}, Price: {body.Price:C}"
        };

        await hubContext.Clients.All.SendAsync("TicketPurchased", notification, ct);
        
        await PublishNotificationSentEvent(body.CustomerId.ToString(), "TicketPurchased",
            "Ticket Purchased", notification.Message, ct);
    }

    private async Task HandleEntryGranted(CloudEvent @event, EntryGrantedEvent body, BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        LogEntryGranted(logger, body.TicketCode, body.CustomerId);

        var notification = new
        {
            Type = "EntryGranted",
            TicketCode = body.TicketCode.ToString(),
            TicketType = body.TicketType,
            GateId = body.GateId.ToString(),
            EntryTime = body.EntryTime,
            CustomerId = body.CustomerId.ToString(),
            EventId = body.EventId.ToString(),
            Message = $"Entry granted at gate {body.GateId}"
        };

        await hubContext.Clients.All.SendAsync("EntryGranted", notification, ct);

        await PublishNotificationSentEvent(body.CustomerId.ToString(), "EntryGranted",
            "Entry Granted", notification.Message, ct);
    }

    private async Task HandleEntryDenied(CloudEvent @event, EntryDeniedEvent body, BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        LogEntryDenied(logger, body.TicketCode,
            body.Reason, body.CustomerId);

        var notification = new
        {
            Type = "EntryDenied",
            TicketCode = body.TicketCode.ToString(),
            GateId = body.GateId.ToString(),
            Reason = body.Reason,
            DeniedTime = body.DeniedTime,
            CustomerId = body.CustomerId.ToString(),
            Message = $"Entry denied: {body.Reason}"
        };

        await hubContext.Clients.All.SendAsync("EntryDenied", notification, ct);

        await PublishNotificationSentEvent(body.CustomerId.ToString(), "EntryDenied",
            "Entry Denied", notification.Message, ct);
    }

    private async Task HandleOccupancyUpdated(CloudEvent @event, OccupancyUpdatedEvent body, BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        LogOccupancyUpdated(logger, body.StageName, body.CurrentOccupancy,
            body.MaxCapacity);

        var notification = new
        {
            Type = "OccupancyUpdated",
            EventId = body.EventId.ToString(),
            StageName = body.StageName,
            CurrentOccupancy = body.CurrentOccupancy,
            MaxCapacity = body.MaxCapacity,
            OccupancyPercentage = body.OccupancyPercentage,
            UpdatedAt = body.UpdatedAt,
            Message =
                $"{body.StageName} occupancy: {body.CurrentOccupancy}/{body.MaxCapacity} ({body.OccupancyPercentage:P0})"
        };

        await hubContext.Clients.All.SendAsync("OccupancyUpdated", notification, ct);

        await PublishBroadcastNotificationEvent("OccupancyUpdated",
            "Occupancy Updated", notification.Message, ct);
    }

    private async Task HandleCapacityWarning(CloudEvent @event, CapacityWarningIssuedEvent body,
        BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        LogCapacityWarning(logger, body.StageName, body.CurrentOccupancy, body.MaxCapacity, body.OccupancyPercentage);

        var notification = new
        {
            Type = "CapacityWarning",
            EventId = body.EventId.ToString(),
            body.StageName,
            body.CurrentOccupancy,
            body.MaxCapacity,
            body.OccupancyPercentage,
            body.WarningThreshold,
            body.IssuedAt,
            Message =
                $"Capacity warning for {body.StageName}: {body.CurrentOccupancy}/{body.MaxCapacity} ({body.OccupancyPercentage:P0})"
        };

        await hubContext.Clients.All.SendAsync("CapacityWarning", notification, ct);

        await PublishBroadcastNotificationEvent("CapacityWarning",
            "Capacity Warning", notification.Message, ct);
    }

    private async Task PublishNotificationSentEvent(string customerId, string notificationType,
        string title, string message, CancellationToken ct)
    {
        var notificationEvent = new NotificationSentEvent
        {
            NotificationId = Guid.NewGuid().ToString(),
            CustomerId = customerId,
            NotificationType = notificationType,
            Title = title,
            Message = message,
            SentAt = DateTime.UtcNow
        };

        await eventBus.PublishMessageAsync(notificationEvent, ct);
    }

    private async Task PublishBroadcastNotificationEvent(string notificationType,
        string title, string message, CancellationToken ct)
    {
        var broadcastEvent = new BroadcastNotificationSentEvent
        {
            NotificationId = Guid.NewGuid().ToString(),
            NotificationType = notificationType,
            Title = title,
            Message = message,
            RecipientCount = -1, // How do i best get this xD?
            SentAt = DateTime.UtcNow
        };

        await eventBus.PublishMessageAsync(broadcastEvent, ct);
    }

    [LoggerMessage(LogLevel.Information,
        "TicketPurchased event received: TicketCode={ticketCode}, CustomerId={customerId}")]
    static partial void LogTicketPurchased(ILogger<QueueWorker> logger, Guid ticketCode, Guid customerId);

    [LoggerMessage(LogLevel.Information,
        "EntryGranted event received: TicketCode={ticketCode}, CustomerId={customerId}")]
    static partial void LogEntryGranted(ILogger<QueueWorker> logger, Guid ticketCode, Guid customerId);

    [LoggerMessage(LogLevel.Information,
        "EntryDenied event received: TicketCode={ticketCode}, Reason={reason}, CustomerId={customerId}")]
    static partial void LogEntryDenied(ILogger<QueueWorker> logger, Guid ticketCode, string reason, Guid customerId);

    [LoggerMessage(LogLevel.Information,
        "OccupancyUpdated event received: Stage={stageName}, Occupancy={current}/{max}")]
    static partial void LogOccupancyUpdated(ILogger<QueueWorker> logger, string stageName, int current, int max);

    [LoggerMessage(LogLevel.Warning,
        "CapacityWarning event received: Stage={stageName}, Occupancy={current}/{max} ({percentage:P0})")]
    static partial void LogCapacityWarning(ILogger<QueueWorker> logger, string stageName, int current, int max,
        double percentage);
}