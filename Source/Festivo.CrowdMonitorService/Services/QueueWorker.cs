using CloudNative.CloudEvents;
using Festivo.CrowdMonitorService.Data;
using Festivo.CrowdMonitorService.Data.Entities;
using Festivo.Shared.Events;
using Festivo.Shared.Services;
using RabbitMQ.Client.Events;

namespace Festivo.CrowdMonitorService.Services;

public class QueueWorker(
    EventBus eventBus,
    IServiceProvider sp) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await eventBus.Initialization.Task;

        await eventBus.AddConsumerAsync<EntryGrantedEvent>("EntryGranted", HandleEntryGranted, stoppingToken);
        await eventBus.AddConsumerAsync<ExitGrantedEvent>("ExitGranted", HandleExitGranted, stoppingToken);
    }

    private async Task HandleEntryGranted(CloudEvent @event, EntryGrantedEvent body, BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        Occupancy? occupancy;

        using (var scope = sp.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CrowdDbContext>();
            
            occupancy = dbContext.Occupancies.FirstOrDefault(o =>
                o.EventId == body.EventId && o.Type == body.TicketType);

            if (occupancy is null)
                return;
        
            occupancy.Current++;
            await dbContext.SaveChangesAsync(ct);
        }

        await eventBus.PublishMessageAsync(new OccupancyUpdatedEvent
        {
            EventId = body.EventId,
            CurrentOccupancy = (int)occupancy.Current,
            MaxCapacity = (int)occupancy.Limit,
            StageName = occupancy.Type,
            UpdatedAt = DateTime.UtcNow,
            OccupancyPercentage = occupancy.Current / (double)occupancy.Limit
        }, ct);

        if (occupancy.Current >= occupancy.WarningThreshold)
        {
            await eventBus.PublishMessageAsync(new CapacityWarningIssuedEvent
            {
                EventId = body.EventId,
                StageName = occupancy.Type,
                CurrentOccupancy = (int)occupancy.Current,
                MaxCapacity = (int)occupancy.Limit,
                OccupancyPercentage = occupancy.Current / (double)occupancy.Limit,
                WarningThreshold = (int)occupancy.WarningThreshold,
                IssuedAt = DateTime.UtcNow
            }
            , ct);
        }
    }

    private async Task HandleExitGranted(CloudEvent @event, ExitGrantedEvent body, BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        Occupancy? occupancy;

        using (var scope = sp.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CrowdDbContext>();
            
            occupancy = dbContext.Occupancies.FirstOrDefault(o =>
                o.EventId == body.EventId && o.Type == body.TicketType);

            if (occupancy is null)
                return;
        
            occupancy.Current--;
            await dbContext.SaveChangesAsync(ct);
        }

        await eventBus.PublishMessageAsync(new OccupancyUpdatedEvent
        {
            EventId = body.EventId,
            CurrentOccupancy = (int)occupancy.Current,
            MaxCapacity = (int)occupancy.Limit,
            StageName = occupancy.Type,
            UpdatedAt = DateTime.UtcNow,
            OccupancyPercentage = occupancy.Current / (double)occupancy.Limit
        }, ct);
    }
}