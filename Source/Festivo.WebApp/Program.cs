using Festivo.AccessControlService.Client.Services;
using Festivo.CrowdMonitorService.Client.Services;
using Festivo.TicketService.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Festivo.WebApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient<TicketClient>(client => client.BaseAddress = new Uri("https://localhost:7181/ticketservice/"));
builder.Services.AddHttpClient<AccessClient>(client => client.BaseAddress = new Uri("https://localhost:7181/accesscontrolservice/"));
builder.Services.AddHttpClient<CrowdClient>(client => client.BaseAddress = new Uri("https://localhost:7181/crowdmonitorservice/"));
builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
