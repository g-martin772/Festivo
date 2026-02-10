using Festivo.Shared.Events;
using Festivo.Shared.Services;
using Festivo.TicketService.Client.Models;
using Festivo.TicketService.Data;
using Festivo.TicketService.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using TicketType = Festivo.TicketService.Client.Models.TicketType;

namespace Festivo.TicketService.Endpoints;

public static class TicketEndpoints
{
    public static void MapTicketEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/purchase", PurchaseHandler);
    }

    private static async Task<IResult> PurchaseHandler(
        [FromBody] PurchaseTicketRequest request,
        [FromServices] EventBus eventBus,
        [FromServices] TicketDbContext dbContext)
    {
        decimal price = 0.0m;

        switch (request.TicketTarget)
        {
            case "concert1":
                switch (request.TicketType)
                {
                    case TicketType.Backstage:
                        price = 100.0m;
                        break;
                    case TicketType.Basic:
                        price = 70.0m;
                        break;
                    case TicketType.VIP:
                        price = 150.0m;
                        break;
                }

                break;
            default:
                return Results.BadRequest();
        }

        var code = Guid.NewGuid();
        var date = DateTime.UtcNow;

        dbContext.Tickets.Add(new Ticket
        {
            Code = code,
            Price = price,
            CustomerId = request.CustomerId,
            PurchaseDate = date,
            State = TicketState.Valid,
            Type = request.TicketType
        });

        await dbContext.SaveChangesAsync();

        await eventBus.PublishMessageAsync(new TicketPurchasedEvent()
        {
            CustomerId = request.CustomerId,
            Price = price,
            PurchaseDate = date,
            TicketCode = code,
            TicketType = request.TicketType,
            EventId = request.EventId
        });

        return Results.Ok(new PurchaseTicketResponse() { Code = code, Price = price });
    }
}