using CloudNative.CloudEvents;
using Festivo.AccessControlService.Data;
using Festivo.AccessControlService.Data.Entities;
using Festivo.Shared.Events;
using Festivo.Shared.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client.Events;

namespace Festivo.AccessControlService.Services;

public partial class QueueWorker(
    EventBus eventBus,
    ILogger<QueueWorker> logger,
    IServiceProvider sp) : BackgroundService
{
    private AccessControlDbContext m_DbContext = null!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = sp.CreateScope();
        // ReSharper disable once InconsistentlySynchronizedField
        m_DbContext = scope.ServiceProvider.GetRequiredService<AccessControlDbContext>();

        await eventBus.Initialization.Task;

        await eventBus.AddConsumerAsync<TicketPurchasedEvent>("TickedPurchased", HandleTicketPurchased, stoppingToken);
        await eventBus.AddConsumerAsync<TicketRefundedEvent>("TickedRefunded", HandleTicketRefunded, stoppingToken);
    }

    private Task HandleTicketPurchased(CloudEvent @event, TicketPurchasedEvent body, BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        LogTickedPurchasedEventReceived(logger, body.TicketCode);

        try
        {
            lock (m_DbContext)
            {
                m_DbContext.ChangeTracker.Clear();
                m_DbContext.ValidTickets.Add(new Ticket
                {
                    Code = body.TicketCode, 
                    Type = body.TicketType,
                    State = State.Out,
                    EventId = body.EventId,
                });
                m_DbContext.SaveChanges();
            }
        }
        catch (DbUpdateException e)
        {
            LogTicketCodeAlreadyExistsInTheDatabaseSkipping(logger, body.TicketCode);
        }

        return Task.CompletedTask;
    }

    private Task HandleTicketRefunded(CloudEvent @event, TicketRefundedEvent body, BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        LogTickedRefundedEventReceived(logger, body.TicketCode);

        lock (m_DbContext)
        {
            m_DbContext.ChangeTracker.Clear();

            var entry = m_DbContext.ValidTickets.Find(body.TicketCode);

            if (entry is null)
            {
                LogTicketCodeDoesNotExistInTheDatabaseSkipping(logger, body.TicketCode);
                return Task.CompletedTask;
            }

            m_DbContext.ValidTickets.Remove(entry);
            m_DbContext.SaveChanges();
        }

        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "Ticked Purchased Event Received: {Code}")]
    static partial void LogTickedPurchasedEventReceived(ILogger<QueueWorker> logger, Guid code);

    [LoggerMessage(LogLevel.Information, "Ticket with code {Code} already exists in the database - Skipping")]
    static partial void LogTicketCodeAlreadyExistsInTheDatabaseSkipping(ILogger<QueueWorker> logger, Guid code);

    [LoggerMessage(LogLevel.Information, "Ticked Refunded Event Received: {Code}")]
    static partial void LogTickedRefundedEventReceived(ILogger<QueueWorker> logger, string code);

    [LoggerMessage(LogLevel.Information, "Ticket with code {Code} does not exist in the database - Skipping")]
    static partial void LogTicketCodeDoesNotExistInTheDatabaseSkipping(ILogger<QueueWorker> logger, string code);
}