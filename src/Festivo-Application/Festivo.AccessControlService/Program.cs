using Festivo.AccessControlService.Services;
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
app.MapGet("/api/access", () => Results.Ok(new { message = "Get all access control entries", service = "AccessControlService" }));
app.MapGet("/api/access/{id}", (string id) => Results.Ok(new { message = $"Get access control entry {id}", service = "AccessControlService" }));
app.MapPost("/api/access/verify", () => Results.Ok(new { message = "Verify access", service = "AccessControlService" }));
app.MapPost("/api/access/grant", () => Results.Ok(new { message = "Grant access", service = "AccessControlService" }));
app.MapPost("/api/access/revoke", () => Results.Ok(new { message = "Revoke access", service = "AccessControlService" }));

app.Run();

#endregion