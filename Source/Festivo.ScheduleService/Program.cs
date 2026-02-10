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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapDefaultEndpoints();

app.Run();