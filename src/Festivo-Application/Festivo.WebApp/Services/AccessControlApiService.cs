using System.Net.Http.Json;
using Festivo.WebApp.Models;
namespace Festivo.WebApp.Services;
public class AccessControlApiService(HttpClient httpClient, IConfiguration configuration)
{
    private readonly string _apiBaseUrl = configuration["ApiGatewayUrl"] ?? "http://localhost:5101";

    public async Task<(bool Success, ScanResponse? Response, string? Error)> ScanTicketAsync(ScanRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/access/scan", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ScanResponse>();
                return (true, result, null);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, null, $"API Error ({response.StatusCode}): {errorMessage}");
            }
        }
        catch (HttpRequestException ex)
        {
            return (false, null, $"Network Error: {ex.Message}. Please ensure the API Gateway is running.");
        }
        catch (Exception ex)
        {
            return (false, null, $"Unexpected Error: {ex.Message}");
        }
    }
}
