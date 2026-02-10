namespace Festivo.Shared.Events;

public class EntryRequestedEvent
{
    public Guid TicketCode { get; set; }
    public Guid GateId { get; set; }
    public DateTime RequestTime { get; set; }
    public Guid CustomerId { get; set; }
}

public class EntryGrantedEvent
{
    public Guid EventId { get; set; }
    public string TicketType { get; set; } = string.Empty;
    public Guid TicketCode { get; set; }
    public Guid GateId { get; set; }
    public DateTime EntryTime { get; set; }
    public Guid CustomerId { get; set; }
}

public class EntryDeniedEvent
{
    public Guid TicketCode { get; set; }
    public Guid GateId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime DeniedTime { get; set; }
    public Guid CustomerId { get; set; }
}

public class ExitRequestedEvent
{
    public Guid TicketCode { get; set; }
    public Guid GateId { get; set; }
    public DateTime RequestTime { get; set; }
    public Guid CustomerId { get; set; }
}

public class ExitGrantedEvent
{
    public Guid EventId { get; set; }
    public string TicketType { get; set; } = string.Empty;
    public Guid TicketCode { get; set; }
    public Guid GateId { get; set; }
    public DateTime ExitTime { get; set; }
    public Guid CustomerId { get; set; }
}

public class ExitDeniedEvent
{
    public Guid TicketCode { get; set; }
    public Guid GateId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime DeniedTime { get; set; }
    public Guid CustomerId { get; set; }
}