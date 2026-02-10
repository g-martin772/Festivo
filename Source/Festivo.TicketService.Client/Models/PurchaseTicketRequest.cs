namespace Festivo.TicketService.Client.Models;

public static class TicketType
{
    public const string Basic = "basic";
    public const string VIP = "vip";
    public const string Backstage = "backstage";
}

public record PurchaseTicketRequest
{
    public Guid EventId { get; set; }
    public Guid CustomerId { get; set; }
    public required string TicketType { get; set; }
    public required string TicketTarget { get; set; }
}