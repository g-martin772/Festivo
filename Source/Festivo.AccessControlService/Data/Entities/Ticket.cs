using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Festivo.AccessControlService.Data.Entities;

public enum State
{
    In,
    Out
}

public class Ticket
{
    public Guid EventId { get; set; }
    public required Guid Code { get; set; }
    public required string Type { get; set; }
    public required State State { get; set; }
}

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("ValidTickets");
        builder.HasKey(t => t.Code);
        builder.Property(t => t.Type)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(t => t.State).IsRequired();
        builder.Property(t => t.EventId).IsRequired();
    }
}