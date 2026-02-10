namespace Festivo.TicketService.Client.Models;

public record PurchaseTicketResponse
{
    public Guid Code { get; set; }
    public decimal Price { get; set; }
}