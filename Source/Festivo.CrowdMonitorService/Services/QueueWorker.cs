using CloudNative.CloudEvents;
using Festivo.CrowdMonitorService.Data;
using Festivo.CrowdMonitorService.Data.Entities;
using Festivo.Shared.Events;
using Festivo.Shared.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client.Events;

namespace Festivo.CrowdMonitorService.Services;

public partial class QueueWorker(
    EventBus eventBus,
    ILogger<QueueWorker> logger,
    IServiceProvider sp) : BackgroundService
{
    private IServiceScope? m_Scope;
    private CrowdDbContext m_DbContext = null!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        m_Scope = sp.CreateScope();
        // ReSharper disable once InconsistentlySynchronizedField
        m_DbContext = m_Scope.ServiceProvider.GetRequiredService<CrowdDbContext>();

        await eventBus.Initialization.Task;

        await eventBus.AddConsumerAsync<EntryGrantedEvent>("EntryGranted", HandleEntryGranted, stoppingToken);
        await eventBus.AddConsumerAsync<ExitGrantedEvent>("ExitGranted", HandleExitGranted, stoppingToken);
    }

    private async Task HandleEntryGranted(CloudEvent @event, EntryGrantedEvent body, BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        Occupancy? occupancy;

        lock (m_DbContext)
        {
            m_DbContext.ChangeTracker.Clear();
            occupancy = m_DbContext.Occupancies.FirstOrDefault(o =>
                o.EventId == body.EventId && o.Type == body.TicketType);

            if (occupancy is null)
                return;
        
            occupancy.Current++;
            m_DbContext.SaveChanges();
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

        lock (m_DbContext)
        {
            m_DbContext.ChangeTracker.Clear();
            occupancy = m_DbContext.Occupancies.FirstOrDefault(o =>
                o.EventId == body.EventId && o.Type == body.TicketType);

            if (occupancy is null)
                return;
        
            occupancy.Current--;
            m_DbContext.SaveChanges();
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