namespace Festivo.WebApp.Models;
public class LiveStatusResponse
{
    public int CurrentCapacity { get; set; }
    public int MaxCapacity { get; set; }
    public List<string>? RecentActivities { get; set; }
}
