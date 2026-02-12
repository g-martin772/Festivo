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
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await eventBus.Initialization.Task;

        await eventBus.AddConsumerAsync<TicketPurchasedEvent>("TickedPurchased", HandleTicketPurchased, stoppingToken);
        await eventBus.AddConsumerAsync<TicketRefundedEvent>("TickedRefunded", HandleTicketRefunded, stoppingToken);
    }

    private async Task HandleTicketPurchased(CloudEvent @event, TicketPurchasedEvent body, BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        LogTickedPurchasedEventReceived(logger, body.TicketCode);

        try
        {
            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AccessControlDbContext>();
            
            dbContext.ValidTickets.Add(new Ticket
            {
                Code = body.TicketCode, 
                Type = body.TicketType,
                State = State.Out,
                EventId = body.EventId,
            });
            
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            LogTicketCodeAlreadyExistsInTheDatabaseSkipping(logger, body.TicketCode);
        }
    }

    private async Task HandleTicketRefunded(CloudEvent @event, TicketRefundedEvent body, BasicDeliverEventArgs args,
        CancellationToken ct)
    {
        LogTickedRefundedEventReceived(logger, body.TicketCode);

        using var scope = sp.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AccessControlDbContext>();

        var entry = await dbContext.ValidTickets.FindAsync([body.TicketCode], cancellationToken: ct);

        if (entry is null)
        {
            LogTicketCodeDoesNotExistInTheDatabaseSkipping(logger, body.TicketCode);
            return;
        }

        dbContext.ValidTickets.Remove(entry);
        await dbContext.SaveChangesAsync(ct);
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