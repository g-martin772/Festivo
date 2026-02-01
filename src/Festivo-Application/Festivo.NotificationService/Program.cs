using Festivo.NotificationService.Services;
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

// Dummy REST endpoints for MS5
app.MapGet("/api/notifications", () => Results.Ok(new { message = "Get all notifications", service = "NotificationService" }));
app.MapGet("/api/notifications/{id}", (string id) => Results.Ok(new { message = $"Get notification {id}", service = "NotificationService" }));
app.MapGet("/api/notifications/user/{userId}", (string userId) => Results.Ok(new { message = $"Get notifications for user {userId}", service = "NotificationService" }));
app.MapPost("/api/notifications/send", () => Results.Ok(new { message = "Send notification", service = "NotificationService" }));
app.MapPut("/api/notifications/{id}/read", (string id) => Results.Ok(new { message = $"Mark notification {id} as read", service = "NotificationService" }));

app.Run();

#endregion