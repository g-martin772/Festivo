namespace Festivo.AccessControlService.Client.Models;

public record ScanResponse
{
    public required bool EntryGranted { get; set; }
    public required string Reason { get; set; }
}