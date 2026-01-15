using Festivo.Shared.Services;
using RabbitMQ.Client;

#region Builder

var builder = WebApplication.CreateBuilder(args);

// RabbitMQ Services
builder.Services.AddSingleton(_ => new ConnectionFactory()
{
    HostName = builder.Configuration["RabbitMQ"] ?? "localhost",
    Port = 5672,
    UserName = "user",
    Password = "password",
});
builder.Services.AddSingleton<IQueueService, RabbitMqQueueService>();

// Logger
builder.Logging
    .ClearProviders()
    .AddConsole();

var app = builder.Build();

#endregion

#region App

_ = app.Services.GetRequiredService<IQueueService>();

app.MapGet("/health", () => Results.Ok());

app.Run();

#endregion