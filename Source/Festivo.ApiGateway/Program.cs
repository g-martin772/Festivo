using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder =>
{
    policyBuilder.AllowAnyOrigin();
    policyBuilder.AllowAnyMethod();
    policyBuilder.AllowAnyHeader();
}));

builder.Services.AddReverseProxy()
    .LoadFromMemory([
        RouteConfigFromService("AccessControlService", useSignalR: true),
        RouteConfigFromService("CallbackService", useSignalR: true),
        RouteConfigFromService("CrowdMonitorService", useSignalR: true),
        RouteConfigFromService("NotificationService", useSignalR: true),
        RouteConfigFromService("OrchestratorService", useSignalR: true),
        RouteConfigFromService("ScheduleService", useSignalR: true),
        RouteConfigFromService("TicketService", useSignalR: true),
    ], [
        ClusterConfigFromService("AccessControlService"),
        ClusterConfigFromService("CallbackService"),
        ClusterConfigFromService("CrowdMonitorService"),
        ClusterConfigFromService("NotificationService"),
        ClusterConfigFromService("OrchestratorService"),
        ClusterConfigFromService("ScheduleService"),
        ClusterConfigFromService("TicketService"),
    ]);

const string? loadBalancingPolicy = "PowerOfTwoChoices";
const string defaultDestinationName = "destination1";

Dictionary<string, DestinationConfig> SingeDestinationConfig(string address) => new()
{
    { defaultDestinationName, new DestinationConfig { Address = address } }
};

ClusterConfig ClusterConfigFromService(string serviceName) => new()
{
    ClusterId = $"{serviceName.ToLower()}_cluster",
    Destinations = SingeDestinationConfig(builder.Configuration[$"services:{serviceName}:https:0"]!),
    LoadBalancingPolicy = loadBalancingPolicy,
    SessionAffinity = new SessionAffinityConfig
    {
        Enabled = true,
        Policy = "HashCookie",
        FailurePolicy = "Redistribute",
        AffinityKeyName = $"{serviceName}Affinity",
        Cookie = new SessionAffinityCookieConfig
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Strict,
            SecurePolicy = CookieSecurePolicy.Always
        }
    }
};

RouteConfig RouteConfigFromService(string serviceName, bool useSignalR = false)
{
    string normalizedServiceName = serviceName.ToLower();

    var transforms = new List<Dictionary<string, string>>
    {
        new() { ["PathRemovePrefix"] = $"/{normalizedServiceName}" },
    };  

    if (useSignalR)
    {
        transforms.Add(new Dictionary<string, string> { ["RequestHeadersCopy"] = "true" });
        transforms.Add(new Dictionary<string, string> { ["RequestHeaderOriginalHost"] = "true" });
        transforms.Add(new Dictionary<string, string> { ["RequestHeaderRemove"] = "Cookie" });
        transforms.Add(new Dictionary<string, string> { ["RequestHeader"] = "Upgrade", ["Set"] = "WebSocket" });
        transforms.Add(new Dictionary<string, string>
        {
            ["RequestHeader"] = "Connection", ["Set"] = "Upgrade"
        });
    }

    var routeConfig = new RouteConfig
    {
        RouteId = $"{normalizedServiceName}_route",
        ClusterId = $"{normalizedServiceName}_cluster",
        Match = new RouteMatch { Path = $"/{normalizedServiceName}/{{**catch-all}}" },
        Transforms = transforms,
        CorsPolicy = "Default"
    };

    return routeConfig;
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapDefaultEndpoints();
app.MapReverseProxy();

app.Run();