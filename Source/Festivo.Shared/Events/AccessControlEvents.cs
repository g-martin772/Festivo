namespace Festivo.Shared.Events;

public class EntryRequestedEvent
{
    public string TicketId { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string GateId { get; set; } = string.Empty;
    public DateTime RequestTime { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}

public class EntryGrantedEvent
{
    public string TicketId { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string GateId { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}

public class EntryDeniedEvent
{
    public string TicketId { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string GateId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime DeniedTime { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}

public class ExitGrantedEvent
{
    public string TicketId { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string GateId { get; set; } = string.Empty;
    public DateTime ExitTime { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}

public class ExitDeniedEvent
{
    public string TicketId { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string GateId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime DeniedTime { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}

