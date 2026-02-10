using Festivo.AccessControlService.Data;
using Festivo.AccessControlService.Endpoints;
using Festivo.AccessControlService.Services;
using Festivo.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddServiceDefaults();

builder.AddRabbitMQClient("RabbitMQ");
builder.Services.AddMessaging([
    ("TickedPurchased", "com.festivo.ticket.purchased.v1"),
    ("TickedRefunded", "com.festivo.ticket.refunded.v1")
]);

builder.AddNpgsqlDbContext<AccessControlDbContext>("AccessControlDb");
builder.Services.AddHostedService<DbInitializer<AccessControlDbContext>>();

builder.Services.AddHostedService<QueueWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapAccessEndpoints();

app.Run();