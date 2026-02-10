using Festivo.AccessControlService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Festivo.AccessControlService.Data;

public class AccessControlDbContext(DbContextOptions<AccessControlDbContext> options) : DbContext(options)
{
    public DbSet<Ticket> ValidTickets { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccessControlDbContext).Assembly);
    }
}