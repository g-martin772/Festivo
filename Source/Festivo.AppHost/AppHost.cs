var builder = DistributedApplication.CreateBuilder(args);

var queue = builder.AddRabbitMQ("RabbitMQ")
    .WithLifetime(ContainerLifetime.Persistent);

var ticketService = builder.AddProject<Projects.Festivo_TicketService>("TicketService")
    .WithReference(queue)
    .WaitFor(queue);

builder.Build().Run();