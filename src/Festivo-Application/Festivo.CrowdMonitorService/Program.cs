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
builder.Services.AddScoped<IQueueService, RabbitMqQueueService>();

// Logger
builder.Services.AddLogging();

var app = builder.Build();

#endregion

#region App

app.MapGet("/health", () => Results.Ok());

app.Run();

#endregion