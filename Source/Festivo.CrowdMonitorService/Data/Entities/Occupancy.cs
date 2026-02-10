using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Festivo.CrowdMonitorService.Data.Entities;

public class Occupancy
{
    public Guid EventId { get; set; }
    public required string Type { get; set; }
    public uint Current { get; set; }
    public uint WarningThreshold { get; set; }
    public uint Limit { get; set; }
}

public class OccupancyConfiguration : IEntityTypeConfiguration<Occupancy>
{
    public void Configure(EntityTypeBuilder<Occupancy> builder)
    {
        builder.ToTable("Occupancies");
        builder.HasKey(t => new {t.EventId, t.Type});
        builder.Property(t => t.Type)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(t => t.Current).IsRequired();
        builder.Property(t => t.WarningThreshold).IsRequired();
        builder.Property(t => t.Limit).IsRequired();
        builder.Property(t => t.EventId).IsRequired();
    }
}