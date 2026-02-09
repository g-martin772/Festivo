using Festivo.Shared.Events;
using Festivo.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.AddServiceDefaults();

builder.AddRabbitMQClient("RabbitMQ");
builder.Services.AddMessaging([
    ("test", "#")
]);

builder.Services.AddHostedService<Demo>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();

class Demo(EventBus eventBus, ILogger<Demo> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(2000, stoppingToken);

        await eventBus.AddConsumerAsync<EntryRequestedEvent>("test", (@event, body, args, ct) =>
        {
            logger.LogInformation("Entry Requested Event Received: {@Content}", body.CustomerId);
            return Task.CompletedTask;
        }, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await eventBus.PublishMessageAsync(new EntryRequestedEvent
            {
                CustomerId = "asd",
                GateId = "asd",
                TicketId = "asd",
                RequestTime = DateTime.Now,
                TicketCode = "asd"
            }, stoppingToken);
            
            await Task.Delay(1000, stoppingToken);
        }
    }
}