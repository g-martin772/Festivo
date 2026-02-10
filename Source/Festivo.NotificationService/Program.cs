using Festivo.NotificationService;
using Festivo.NotificationService.Hubs;
using Festivo.NotificationService.Services;
using Festivo.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddServiceDefaults();

builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder =>
{
    policyBuilder.AllowAnyOrigin();
    policyBuilder.AllowAnyMethod();
    policyBuilder.AllowAnyHeader();
}));

builder.AddRabbitMQClient("RabbitMQ");
builder.Services.AddMessaging([
    ("NotificationForTicketPurchased", "com.festivo.ticket.purchased.v1"),
    ("NotificationForEntryGranted", "com.festivo.access.entry-granted.v1"),
    ("NotificationForEntryDenied", "com.festivo.access.entry-denied.v1"),
    ("NotificationForOccupancyUpdated", "com.festivo.crowd.occupancy-updated.v1"),
    ("NotificationForCapacityWarning", "com.festivo.crowd.capacity-warning.v1")
]);

builder.Services.AddSignalR();
builder.Services.AddHostedService<QueueWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapDefaultEndpoints();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
