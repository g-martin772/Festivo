using Festivo.Shared.Events;
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
    ("test", "#")
]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapDefaultEndpoints();

app.Run();
