using Festivo.AccessControlService.Client.Models;
using Festivo.AccessControlService.Data;
using Festivo.AccessControlService.Data.Entities;
using Festivo.Shared.Events;
using Festivo.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace Festivo.AccessControlService.Endpoints;

public static class AccessEndpoints
{
    public static void MapAccessEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/scan-entry", EntryScanHandler);
        routes.MapPost("/scan-exit", ExitScanHandler);
    }

    private static async Task<IResult> EntryScanHandler(
        [FromBody] ScanRequest request,
        [FromServices] EventBus eventBus,
        [FromServices] AccessControlDbContext dbContext)
    {
        var reason = "Invalid Code";
        var time = DateTime.UtcNow;

        var t1 = eventBus.PublishMessageAsync(new EntryRequestedEvent
        {
            CustomerId = request.CustomerId,
            TicketCode = request.TicketCode,
            GateId = request.GateId,
            RequestTime = time
        });

        // validate code and signature....
        
        if (request.GateId != Guid.Parse("00000000-0000-0000-0000-000000000001"))
        {
            reason = "Gate is not operational.";
            var t2 = eventBus.PublishMessageAsync(new EntryDeniedEvent
            {
                CustomerId = request.CustomerId,
                TicketCode = request.TicketCode,
                GateId = request.GateId,
                Reason = reason,
                DeniedTime = time
            });

            await Task.WhenAll(t1, t2);
            return Results.Ok(new ScanResponse { EntryGranted = false, Reason = reason });
        }

        var entry = await dbContext.ValidTickets.FindAsync(request.TicketCode);

        if (entry is null)
        {
            var t2 = eventBus.PublishMessageAsync(new EntryDeniedEvent
            {
                CustomerId = request.CustomerId,
                TicketCode = request.TicketCode,
                GateId = request.GateId,
                Reason = reason,
                DeniedTime = time
            });

            await Task.WhenAll(t1, t2);
            return Results.Ok(new ScanResponse { EntryGranted = false, Reason = reason });
        }

        if (entry.EventId != request.EventId)
        {
            reason = "Ticket not valid for this event.";
            var t2 = eventBus.PublishMessageAsync(new EntryDeniedEvent
            {
                CustomerId = request.CustomerId,
                TicketCode = request.TicketCode,
                GateId = request.GateId,
                Reason = reason,
                DeniedTime = time
            });

            await Task.WhenAll(t1, t2);
            return Results.Ok(new ScanResponse { EntryGranted = false, Reason = reason });
        }
        
        if (entry.State == State.In)
        {
            reason = "Ticket already used for entry.";
            var t2 = eventBus.PublishMessageAsync(new EntryDeniedEvent
            {
                CustomerId = request.CustomerId,
                TicketCode = request.TicketCode,
                GateId = request.GateId,
                Reason = reason,
                DeniedTime = time
            });

            await Task.WhenAll(t1, t2);
            return Results.Ok(new ScanResponse { EntryGranted = false, Reason = reason });
        }
        
        reason = "Ticked verified";
        var t3 = eventBus.PublishMessageAsync(new EntryGrantedEvent
        {
            CustomerId = request.CustomerId,
            TicketCode = request.TicketCode,
            GateId = request.GateId,
            EntryTime = time
        });
        
        entry.State = State.In;
        var t4 = dbContext.SaveChangesAsync();

        await Task.WhenAll(t1, t3, t4);

        return Results.Ok(new ScanResponse { EntryGranted = true, Reason = reason });
    }
    
    private static async Task<IResult> ExitScanHandler(
        [FromBody] ScanRequest request,
        [FromServices] EventBus eventBus,
        [FromServices] AccessControlDbContext dbContext)
    {
        var reason = "Invalid Code";
        var time = DateTime.UtcNow;

        var t1 = eventBus.PublishMessageAsync(new ExitRequestedEvent
        {
            CustomerId = request.CustomerId,
            TicketCode = request.TicketCode,
            GateId = request.GateId,
            RequestTime = time
        });

        // validate code and signature....
        
        if (request.GateId != Guid.Parse("00000000-0000-0000-0000-000000000001"))
        {
            reason = "Gate is not operational.";
            var t2 = eventBus.PublishMessageAsync(new ExitDeniedEvent
            {
                CustomerId = request.CustomerId,
                TicketCode = request.TicketCode,
                GateId = request.GateId,
                Reason = reason,
                DeniedTime = time
            });

            await Task.WhenAll(t1, t2);
            return Results.Ok(new ScanResponse { EntryGranted = false, Reason = reason });
        }

        var entry = await dbContext.ValidTickets.FindAsync(request.TicketCode);

        if (entry is null)
        {
            var t2 = eventBus.PublishMessageAsync(new ExitDeniedEvent
            {
                CustomerId = request.CustomerId,
                TicketCode = request.TicketCode,
                GateId = request.GateId,
                Reason = reason,
                DeniedTime = time
            });

            await Task.WhenAll(t1, t2);
            return Results.Ok(new ScanResponse { EntryGranted = false, Reason = reason });
        }

        if (entry.EventId != request.EventId)
        {
            reason = "Ticket not valid for this event.";
            var t2 = eventBus.PublishMessageAsync(new ExitDeniedEvent
            {
                CustomerId = request.CustomerId,
                TicketCode = request.TicketCode,
                GateId = request.GateId,
                Reason = reason,
                DeniedTime = time
            });

            await Task.WhenAll(t1, t2);
            return Results.Ok(new ScanResponse { EntryGranted = false, Reason = reason });
        }
        
        if (entry.State == State.Out)
        {
            reason = "Not inside of event area.";
            var t2 = eventBus.PublishMessageAsync(new ExitDeniedEvent
            {
                CustomerId = request.CustomerId,
                TicketCode = request.TicketCode,
                GateId = request.GateId,
                Reason = reason,
                DeniedTime = time
            });

            await Task.WhenAll(t1, t2);
            return Results.Ok(new ScanResponse { EntryGranted = false, Reason = reason });
        }
        
        reason = "Ticked verified";
        var t3 = eventBus.PublishMessageAsync(new ExitGrantedEvent
        {
            CustomerId = request.CustomerId,
            TicketCode = request.TicketCode,
            GateId = request.GateId,
            ExitTime = time
        });
        
        entry.State = State.Out;
        var t4 = dbContext.SaveChangesAsync();

        await Task.WhenAll(t1, t3, t4);

        return Results.Ok(new ScanResponse { EntryGranted = true, Reason = reason });
    }
}