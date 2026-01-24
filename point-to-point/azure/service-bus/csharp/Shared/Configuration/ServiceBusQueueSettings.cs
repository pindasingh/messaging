namespace Shared.Configuration;

public class ServiceBusQueueSettings
{
    public const string SectionName = "ServiceBus";

    public string ConnectionString { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
}
