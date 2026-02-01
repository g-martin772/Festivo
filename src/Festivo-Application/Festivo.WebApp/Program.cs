using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Festivo.WebApp;
using Festivo.WebApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register API Services with Configuration
builder.Services.AddScoped(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var config = sp.GetRequiredService<IConfiguration>();
    return new TicketApiService(httpClient, config);
});

builder.Services.AddScoped(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var config = sp.GetRequiredService<IConfiguration>();
    return new AccessControlApiService(httpClient, config);
});

builder.Services.AddScoped(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var config = sp.GetRequiredService<IConfiguration>();
    return new StatusApiService(httpClient, config);
});

await builder.Build().RunAsync();
