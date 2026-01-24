namespace Shared.Configuration;

public class TasksSettings
{
    public const string SectionName = "Tasks";

    public string ProjectId { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public string QueueId { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
}
