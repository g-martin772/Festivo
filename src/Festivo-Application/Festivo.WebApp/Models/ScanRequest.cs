namespace Festivo.WebApp.Models;
public class ScanRequest
{
    public string? TicketCode { get; set; }
    public string? Action { get; set; } // "entry" or "exit"
}
