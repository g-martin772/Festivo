using System.Text;
using Festivo.CrowdMonitorService.Data;
using Festivo.CrowdMonitorService.Data.Entities;
using Festivo.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Festivo.CrowdMonitorService.Endpoints;

public static class CrowdControlEndpoints
{
    public static void MapCrowdControlEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/", GetStateHandler);
    }

    private static async Task<IResult> GetStateHandler(
        [FromServices] EventBus eventBus,
        [FromServices] CrowdDbContext dbContext)
    {
        var occupancies = await dbContext.Occupancies.ToListAsync();
        
        var summary = new StringBuilder();
        summary.AppendLine("Occupancy Summary:");
        summary.AppendLine("==================");
        summary.AppendLine();
        
        foreach (var occupancy in occupancies)
        {
            var percentage = occupancy.Limit > 0 
                ? (double)occupancy.Current / occupancy.Limit * 100 
                : 0;
            
            summary.AppendLine($"Event: {occupancy.EventId}");
            summary.AppendLine($"Type: {occupancy.Type}");
            summary.AppendLine($"Current: {occupancy.Current} / {occupancy.Limit} ({percentage:F1}%)");
            summary.AppendLine($"Warning Threshold: {occupancy.WarningThreshold}");
            summary.AppendLine($"Status: {GetStatus(occupancy)}");
            summary.AppendLine("---");
        }

        return Results.Text(summary.ToString(), "text/plain");
    }
    
    private static string GetStatus(Occupancy occupancy)
    {
        if (occupancy.Current >= occupancy.Limit)
            return "AT CAPACITY";
        if (occupancy.Current >= occupancy.WarningThreshold)
            return "WARNING";
        return "OK";
    }
}