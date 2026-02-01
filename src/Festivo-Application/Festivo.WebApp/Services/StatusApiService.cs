using System.Net.Http.Json;
using Festivo.WebApp.Models;
namespace Festivo.WebApp.Services;
public class StatusApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;
    public StatusApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiBaseUrl = configuration["ApiGatewayUrl"] ?? "http://localhost:5101";
    }
    public async Task<(bool Success, LiveStatusResponse? Response, string? Error)> GetLiveStatusAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/status/live");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LiveStatusResponse>();
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
