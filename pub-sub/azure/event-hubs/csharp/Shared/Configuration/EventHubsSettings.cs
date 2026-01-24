namespace Shared.Configuration;

public class EventHubsSettings
{
    public const string SectionName = "EventHubs";

    public string ConnectionString { get; set; } = string.Empty;
    public string EventHubName { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = "$Default";
}
