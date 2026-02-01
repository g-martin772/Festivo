namespace Festivo.Shared.Events;

public class TicketPurchasedEvent
{
    public string TicketId { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string TicketType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}

public class TicketRefundedEvent
{
    public string TicketId { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public DateTime RefundDate { get; set; }
}

public class TicketCancelledEvent
{
    public string TicketId { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CancellationDate { get; set; }
}

