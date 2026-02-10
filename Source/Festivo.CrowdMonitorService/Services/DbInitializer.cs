using System.Diagnostics;
using Festivo.CrowdMonitorService.Data;
using Festivo.CrowdMonitorService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Festivo.CrowdMonitorService.Services;

public class DbInitializer<T>(IServiceProvider sp) : BackgroundService where T : DbContext
{
    private readonly ActivitySource m_ActivitySource = new("Festivo.AccessControlService.DbInitializer");
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var activity = m_ActivitySource.StartActivity();
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        await context.Database.MigrateAsync(cancellationToken: stoppingToken);

        if (context is CrowdDbContext crowdDbContext && !(await crowdDbContext.Occupancies.AnyAsync(cancellationToken: stoppingToken)))
        {
            crowdDbContext.Occupancies.AddRange([
                new Occupancy
                {
                    EventId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Type = "basic",
                    Current = 0,
                    WarningThreshold = 80,
                    Limit = 100
                },
                new Occupancy
                {
                    EventId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Type = "vip",
                    Current = 0,
                    WarningThreshold = 40,
                    Limit = 50
                },
                new Occupancy
                {
                    EventId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Type = "backstage",
                    Current = 0,
                    WarningThreshold = 2,
                    Limit = 5
                }
            ]);
            
            await crowdDbContext.SaveChangesAsync(stoppingToken);
        }
        
        activity?.Stop();
    }
}