using Festivo.ApiGateway.Services;
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

// YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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

// Map YARP reverse proxy
app.MapReverseProxy();

app.Run();

#endregion

