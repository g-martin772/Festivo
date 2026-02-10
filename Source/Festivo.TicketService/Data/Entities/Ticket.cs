using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Festivo.TicketService.Data.Entities;

public class Ticket
{
    public required Guid Code { get; set; } 
    public required string Type { get; set; }
    public required string State { get; set; }
    public required decimal Price { get; set; }
    public required DateTime PurchaseDate { get; set; }
    public required Guid CustomerId { get; set; }
}

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");
        builder.HasKey(t => t.Code);
        builder.Property(t => t.Type)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(t => t.State)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(t => t.Price).IsRequired();
        builder.Property(t => t.PurchaseDate).IsRequired();
        builder.Property(t => t.CustomerId).IsRequired();
        builder.HasIndex(t => t.CustomerId);
    }
}