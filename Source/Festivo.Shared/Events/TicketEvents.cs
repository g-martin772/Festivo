namespace Festivo.Shared.Events;

public static class TicketState
{
    public const string Pending = "pending";
    public const string Valid = "valid";
    public const string Expired = "expired";
    public const string Refunded = "refunded";
}

public static class TicketType
{
    public const string Basic = "basic";
    public const string VIP = "vip";
    public const string Backstage = "backstage";
}

public class TicketPurchasedEvent
{
    public Guid EventId { get; set; }
    public Guid TicketCode { get; set; }
    public string TicketType { get; set; } = Events.TicketType.Basic;
    public decimal Price { get; set; }
    public DateTime PurchaseDate { get; set; }
    public Guid CustomerId { get; set; }
}

public class TicketRefundedEvent
{
    public string TicketCode { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public DateTime RefundDate { get; set; }
}

public class TicketCancelledEvent
{
    public string TicketCode { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CancellationDate { get; set; }
}

