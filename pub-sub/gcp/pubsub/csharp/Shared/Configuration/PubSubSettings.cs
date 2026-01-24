namespace Shared.Configuration;

public class PubSubSettings
{
    public const string SectionName = "PubSub";

    public string ProjectId { get; set; } = string.Empty;
    public string TopicId { get; set; } = string.Empty;
    public string SubscriptionId { get; set; } = string.Empty;
}
