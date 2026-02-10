var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("db-username");
var password = builder.AddParameter("db-pw");

var cache = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithRedisCommander();

var postgres = builder.AddPostgres("PostgresDb", username, password)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithPgWeb(options => { options.WithLifetime(ContainerLifetime.Persistent); });

var accessControlDb = postgres.AddDatabase("AccessControlDb");
var crowdMonitorDb = postgres.AddDatabase("CrowdMonitorDb");
var ticketDb = postgres.AddDatabase("TicketDb");

var queue = builder.AddRabbitMQ("RabbitMQ")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithManagementPlugin();

var ticketService = builder.AddProject<Projects.Festivo_TicketService>("TicketService")
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(ticketDb)
    .WaitFor(ticketDb);

var callbackService = builder.AddProject<Projects.Festivo_CallbackService>("CallbackService")
    .WithReference(queue)
    .WaitFor(queue);

var accessControlService = builder.AddProject<Projects.Festivo_AccessControlService>("AccessControlService")
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(accessControlDb)
    .WaitFor(accessControlDb);

var crowdMonitorService = builder.AddProject<Projects.Festivo_CrowdMonitorService>("CrowdMonitorService")
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(crowdMonitorDb)
    .WaitFor(crowdMonitorDb);

var notificationService = builder.AddProject<Projects.Festivo_NotificationService>("NotificationService")
    .WithReference(queue)
    .WaitFor(queue);

var orchestratorService = builder.AddProject<Projects.Festivo_OrchestratorService>("OrchestratorService")
    .WithReference(queue)
    .WaitFor(queue);

var scheduleService = builder.AddProject<Projects.Festivo_ScheduleService>("ScheduleService")
    .WithReference(queue)
    .WaitFor(queue);

var gateway = builder.AddProject<Projects.Festivo_ApiGateway>("ApiGateway")
    .WithReference(callbackService)
    .WithReference(ticketService)
    .WithReference(accessControlService)
    .WithReference(crowdMonitorService)
    .WithReference(notificationService)
    .WithReference(scheduleService)
    .WithReference(orchestratorService);


var webApp = builder.AddProject<Projects.Festivo_WebApp>("WebApp", launchProfileName: "http")
    .WithReference(gateway);

builder.Build().Run();