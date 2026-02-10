namespace Festivo.AccessControlService.Client.Models;

public static class GateType
{
    public const string Basic = "basic";
    public const string VIP = "vip";
    public const string Backstage = "backstage";
}

public record ScanRequest
{
    public required Guid EventId { get; set; }
    public required Guid TicketCode { get; set; }
    public required Guid CustomerId { get; set; }
    public required string GateType { get; set; }
    public required Guid GateId { get; set; }
}