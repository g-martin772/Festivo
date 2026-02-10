using Festivo.Shared.Events;
using Festivo.Shared.Services;
using Festivo.TicketService.Client.Models;
using Festivo.TicketService.Data;
using Festivo.TicketService.Data.Entities;
using Festivo.TicketService.Endpoints;
using Festivo.TicketService.Services;
using Microsoft.AspNetCore.Mvc;
using TicketType = Festivo.TicketService.Client.Models.TicketType;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder =>
{
    policyBuilder.AllowAnyOrigin();
    policyBuilder.AllowAnyMethod();
    policyBuilder.AllowAnyHeader();
}));

builder.AddServiceDefaults();

builder.AddRabbitMQClient("RabbitMQ");
builder.Services.AddMessaging([
    ("test", "#")
]);

builder.AddNpgsqlDbContext<TicketDbContext>("TicketDb");
builder.Services.AddHostedService<DbInitializer<TicketDbContext>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.MapDefaultEndpoints();
app.MapTicketEndpoints();

app.Run();

/*

   Functional workflow (minimum)

       Purchase ticket in Ticket Service
       Ticket Service publishes TicketPurchased
       Access Control consumes TicketPurchased and registers the ticket code
       Entry scan triggers EntryRequested and results in EntryGranted or EntryDenied
       Crowd Monitor consumes EntryGranted / ExitGranted and updates occupancy

   Requirements

       TicketService:
           Must generate a ticket code (string) returned to client
           Must store ticket state in memory for now (DB later)
       AccessControlService:
           Must reject unknown/invalid/refunded tickets
           Must enforce “no double entry” (enter twice without exit → denied)
           Must write scan decisions with a reason
       CrowdMonitorService:
           Must track occupancy per stage/area (choose a simple model)
           Must publish OccupancyUpdated when occupancy changes
       All inter-service updates must happen via RabbitMQ events (not direct calls)
   */

