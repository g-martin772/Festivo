using System.Net.Http.Json;
using Festivo.AccessControlService.Client.Models;

namespace Festivo.AccessControlService.Client.Services;

public class AccessClient(HttpClient client)
{
    public async Task<ScanResponse> ScanTicketAsync(ScanRequest request)
    {
        var response = await client.PostAsJsonAsync("/scan", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ScanResponse>() ?? throw new Exception("Failed to parse purchase ticket response.");
    }
}