using Festivo.CrowdMonitorService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Festivo.CrowdMonitorService.Data;

public class CrowdDbContext(DbContextOptions<CrowdDbContext> options) : DbContext(options)
{
    public DbSet<Occupancy> Occupancies { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrowdDbContext).Assembly);
    }
}