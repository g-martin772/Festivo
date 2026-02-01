using Festivo.ScheduleService.Services;
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
app.MapGet("/api/schedule", () => Results.Ok(new { message = "Get all schedules", service = "ScheduleService" }));
app.MapGet("/api/schedule/{id}", (string id) => Results.Ok(new { message = $"Get schedule {id}", service = "ScheduleService" }));
app.MapGet("/api/schedule/event/{eventId}", (string eventId) => Results.Ok(new { message = $"Get schedule for event {eventId}", service = "ScheduleService" }));
app.MapPost("/api/schedule", () => Results.Ok(new { message = "Create schedule", service = "ScheduleService" }));
app.MapPut("/api/schedule/{id}", (string id) => Results.Ok(new { message = $"Update schedule {id}", service = "ScheduleService" }));

app.Run();

#endregion