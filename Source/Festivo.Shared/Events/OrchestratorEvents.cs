namespace Festivo.Shared.Events;

public class SagaStartedEvent
{
    public string SagaId { get; set; } = string.Empty;
    public string SagaType { get; set; } = string.Empty;
    public string TriggerEventType { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
}

public class SagaCompletedEvent
{
    public string SagaId { get; set; } = string.Empty;
    public string SagaType { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
}

public class SagaFailedEvent
{
    public string SagaId { get; set; } = string.Empty;
    public string SagaType { get; set; } = string.Empty;
    public string ErrorReason { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
}

public class CompensationTriggeredEvent
{
    public string SagaId { get; set; } = string.Empty;
    public string SagaType { get; set; } = string.Empty;
    public string CompensationAction { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime TriggeredAt { get; set; }
}

public class RefundRequestedEvent
{
    public string TicketId { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public DateTime RequestedAt { get; set; }
}

