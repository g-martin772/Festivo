using System.Net.Http.Json;
using Festivo.TicketService.Client.Models;

namespace Festivo.TicketService.Client.Services;

public class TicketClient(HttpClient client)
{
    public async Task<PurchaseTicketResponse> PurchaseTicketAsync(PurchaseTicketRequest request)
    {
        Console.WriteLine(client.BaseAddress);
        var response = await client.PostAsJsonAsync("https://localhost:7181/ticketservice/purchase", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PurchaseTicketResponse>() ?? throw new Exception("Failed to parse purchase ticket response.");
    }
}