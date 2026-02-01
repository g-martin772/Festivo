using System.Net.Http.Json;
using Festivo.WebApp.Models;
namespace Festivo.WebApp.Services;
public class TicketApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;
    public TicketApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiBaseUrl = configuration["ApiGatewayUrl"] ?? "http://localhost:5101";
    }
    public async Task<(bool Success, PurchaseTicketResponse? Response, string? Error)> PurchaseTicketAsync(PurchaseTicketRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/tickets/purchase", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PurchaseTicketResponse>();
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
