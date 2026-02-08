using System.Text.Json;

namespace Festivo.Shared.Events;

public static class EventTypeRegistry
{
    private static readonly Dictionary<string, Type> s_EventTypeMap = new()
    {
        { "com.festivo.ticket.purchased.v1", typeof(TicketPurchasedEvent) },
        { "com.festivo.ticket.refunded.v1", typeof(TicketRefundedEvent) },
        { "com.festivo.ticket.cancelled.v1", typeof(TicketCancelledEvent) },
        
        { "com.festivo.access.entry-requested.v1", typeof(EntryRequestedEvent) },
        { "com.festivo.access.entry-granted.v1", typeof(EntryGrantedEvent) },
        { "com.festivo.access.entry-denied.v1", typeof(EntryDeniedEvent) },
        { "com.festivo.access.exit-granted.v1", typeof(ExitGrantedEvent) },
        { "com.festivo.access.exit-denied.v1", typeof(ExitDeniedEvent) },
        
        { "com.festivo.schedule.item-created.v1", typeof(ScheduleItemCreatedEvent) },
        { "com.festivo.schedule.item-updated.v1", typeof(ScheduleItemUpdatedEvent) },
        { "com.festivo.schedule.item-deleted.v1", typeof(ScheduleItemDeletedEvent) },
        
        { "com.festivo.crowd.occupancy-updated.v1", typeof(OccupancyUpdatedEvent) },
        { "com.festivo.crowd.capacity-warning.v1", typeof(CapacityWarningIssuedEvent) },
        { "com.festivo.crowd.capacity-critical.v1", typeof(CapacityCriticalIssuedEvent) },
        { "com.festivo.crowd.capacity-normal.v1", typeof(CapacityBackToNormalEvent) },
        
        { "com.festivo.notification.sent.v1", typeof(NotificationSentEvent) },
        { "com.festivo.notification.broadcast.v1", typeof(BroadcastNotificationSentEvent) },
        
        { "com.festivo.saga.started.v1", typeof(SagaStartedEvent) },
        { "com.festivo.saga.completed.v1", typeof(SagaCompletedEvent) },
        { "com.festivo.saga.failed.v1", typeof(SagaFailedEvent) },
        { "com.festivo.saga.compensation-triggered.v1", typeof(CompensationTriggeredEvent) },
        { "com.festivo.saga.refund-requested.v1", typeof(RefundRequestedEvent) }
    };

    public static Type? GetEventType(string eventTypeName)
    {
        return s_EventTypeMap.GetValueOrDefault(eventTypeName);
    }

    public static string GetEventTypeName(Type eventType)
    {
        var entry = s_EventTypeMap.FirstOrDefault(x => x.Value == eventType);
        return entry.Key ?? throw new InvalidOperationException($"Event type {eventType.Name} is not registered");
    }

    public static object? DeserializeEventData(string eventTypeName, string jsonData)
    {
        var eventType = GetEventType(eventTypeName);
        if (eventType == null)
            return null;

        return JsonSerializer.Deserialize(jsonData, eventType);
    }
}

