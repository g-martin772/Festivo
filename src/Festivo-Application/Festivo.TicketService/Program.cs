using Festivo.TicketService.Services;
using RabbitMQ.Client;

#region Builder

var builder = WebApplication.CreateBuilder(args);

// RabbitMQ Services
builder.Services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory()
{
    HostName = builder.Configuration["RabbitMQ"] ?? "localhost",
    Port = 5672,
    UserName = "user",
    Password = "password",
});

// Logger
builder.Logging
    .ClearProviders()
    .AddConsole();

// Background Service
builder.Services.AddHostedService<QueueBackgroundService>();

var app = builder.Build();

#endregion

#region App

app.MapGet("/health", () => Results.Ok());

// Dummy endpoints for MS5
app.MapGet("/api/tickets", () => Results.Ok(new { message = "Get all tickets", service = "TicketService" }));
app.MapGet("/api/tickets/{id}", (string id) => Results.Ok(new { message = $"Get ticket {id}", service = "TicketService" }));
app.MapPost("/api/tickets", () => Results.Ok(new { message = "Create ticket", service = "TicketService" }));
app.MapPut("/api/tickets/{id}", (string id) => Results.Ok(new { message = $"Update ticket {id}", service = "TicketService" }));
app.MapDelete("/api/tickets/{id}", (string id) => Results.Ok(new { message = $"Delete ticket {id}", service = "TicketService" }));

app.Run();

#endregion