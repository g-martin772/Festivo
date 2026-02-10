using System.Net.Http.Json;
using Festivo.AccessControlService.Client.Models;

namespace Festivo.AccessControlService.Client.Services;

public class AccessClient(HttpClient client)
{
    public async Task<ScanResponse> ScanEntryTicketAsync(ScanRequest request)
    {
        var response = await client.PostAsJsonAsync("scan-entry", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ScanResponse>() ??
               throw new Exception("Failed to parse purchase ticket response.");
    }

    public async Task<ScanResponse> ScanExitTicketAsync(ScanRequest request)
    {
        var response = await client.PostAsJsonAsync("scan-exit", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ScanResponse>() ??
               throw new Exception("Failed to parse purchase ticket response.");
    }
}