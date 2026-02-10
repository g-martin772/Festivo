using Festivo.CrowdMonitorService.Data;
using Festivo.CrowdMonitorService.Endpoints;
using Festivo.CrowdMonitorService.Services;
using Festivo.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder =>
{
    policyBuilder.AllowAnyOrigin();
    policyBuilder.AllowAnyMethod();
    policyBuilder.AllowAnyHeader();
}));

builder.AddServiceDefaults();

builder.AddRabbitMQClient("RabbitMQ");
builder.Services.AddMessaging([
    ("EntryGranted", "com.festivo.access.entry-granted.v1"),
    ("ExitGranted", "com.festivo.access.exit-granted.v1")
]);

builder.AddNpgsqlDbContext<CrowdDbContext>("CrowdMonitorDb");
builder.Services.AddHostedService<DbInitializer<CrowdDbContext>>();

builder.Services.AddHostedService<QueueWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapDefaultEndpoints();
app.MapCrowdControlEndpoints();

app.Run();
