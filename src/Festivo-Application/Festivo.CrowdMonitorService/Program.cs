using Festivo.CrowdMonitorService.Services;
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
app.MapGet("/api/crowd", () => Results.Ok(new { message = "Get all crowd data", service = "CrowdMonitorService" }));
app.MapGet("/api/crowd/location/{locationId}", (string locationId) => Results.Ok(new { message = $"Get crowd data for location {locationId}", service = "CrowdMonitorService" }));
app.MapGet("/api/crowd/stats", () => Results.Ok(new { message = "Get crowd statistics", service = "CrowdMonitorService" }));
app.MapPost("/api/crowd/update", () => Results.Ok(new { message = "Update crowd data", service = "CrowdMonitorService" }));

app.Run();

#endregion