namespace Festivo.Shared.Events;

public class OccupancyUpdatedEvent
{
    public string StageId { get; set; } = string.Empty;
    public string StageName { get; set; } = string.Empty;
    public int CurrentOccupancy { get; set; }
    public int MaxCapacity { get; set; }
    public double OccupancyPercentage { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CapacityWarningIssuedEvent
{
    public string StageId { get; set; } = string.Empty;
    public string StageName { get; set; } = string.Empty;
    public int CurrentOccupancy { get; set; }
    public int MaxCapacity { get; set; }
    public double OccupancyPercentage { get; set; }
    public int WarningThreshold { get; set; }
    public DateTime IssuedAt { get; set; }
}

public class CapacityCriticalIssuedEvent
{
    public string StageId { get; set; } = string.Empty;
    public string StageName { get; set; } = string.Empty;
    public int CurrentOccupancy { get; set; }
    public int MaxCapacity { get; set; }
    public double OccupancyPercentage { get; set; }
    public int CriticalThreshold { get; set; }
    public DateTime IssuedAt { get; set; }
}

public class CapacityBackToNormalEvent
{
    public string StageId { get; set; } = string.Empty;
    public string StageName { get; set; } = string.Empty;
    public int CurrentOccupancy { get; set; }
    public int MaxCapacity { get; set; }
    public double OccupancyPercentage { get; set; }
    public DateTime NormalizedAt { get; set; }
}

