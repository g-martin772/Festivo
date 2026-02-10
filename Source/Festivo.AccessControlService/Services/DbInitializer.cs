using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Festivo.AccessControlService.Services;

public class DbInitializer<T>(IServiceProvider sp) : BackgroundService where T : DbContext
{
    private readonly ActivitySource m_ActivitySource = new("Festivo.AccessControlService.DbInitializer");
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var activity = m_ActivitySource.StartActivity();
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        await context.Database.MigrateAsync(cancellationToken: stoppingToken);
        activity?.Stop();
    }
}