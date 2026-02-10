namespace Festivo.CrowdMonitorService.Client.Services;

public class CrowdClient(HttpClient client)
{
    public async Task<string> GetData()
    {
        Console.WriteLine(client.BaseAddress);
        var response = await client.GetAsync("");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync() ?? throw new Exception("Failed to parse response.");
    }
}