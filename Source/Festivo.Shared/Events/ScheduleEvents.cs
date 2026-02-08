namespace Festivo.Shared.Events;

public class ScheduleItemCreatedEvent
{
    public string ItemId { get; set; } = string.Empty;
    public string StageId { get; set; } = string.Empty;
    public string StageName { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ScheduleItemUpdatedEvent
{
    public string ItemId { get; set; } = string.Empty;
    public string StageId { get; set; } = string.Empty;
    public string StageName { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ScheduleItemDeletedEvent
{
    public string ItemId { get; set; } = string.Empty;
    public string StageId { get; set; } = string.Empty;
    public DateTime DeletedAt { get; set; }
}

