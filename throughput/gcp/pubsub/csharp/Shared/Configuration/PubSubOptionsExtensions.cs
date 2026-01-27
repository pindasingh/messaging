using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Configuration;

public static class PubSubOptionsExtensions
{
    public static OptionsBuilder<PubSubSettings> AddPubSubSettings(
        this IServiceCollection services,
        IConfiguration configuration,
        bool requireTopic = false,
        bool requireSubscription = false)
    {
        return services.AddOptions<PubSubSettings>()
            .Bind(configuration.GetSection(PubSubSettings.SectionName))
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.ProjectId), "PubSub:ProjectId is required.")
            .Validate(settings => !requireTopic || !string.IsNullOrWhiteSpace(settings.TopicId), "PubSub:TopicId is required.")
            .Validate(settings => !requireSubscription || !string.IsNullOrWhiteSpace(settings.SubscriptionId), "PubSub:SubscriptionId is required.")
            .ValidateOnStart();
    }

    public static OptionsBuilder<ThroughputSettings> AddThroughputSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddOptions<ThroughputSettings>()
            .Bind(configuration.GetSection(ThroughputSettings.SectionName))
            .Validate(settings => settings.MaxConcurrentMessages > 0, "Throughput:MaxConcurrentMessages must be > 0.")
            .Validate(settings => settings.MaxOutstandingMessageCount > 0, "Throughput:MaxOutstandingMessageCount must be > 0.")
            .Validate(settings => settings.MaxOutstandingBytes > 0, "Throughput:MaxOutstandingBytes must be > 0.")
            .Validate(settings => settings.PublishWorkerCount > 0, "Throughput:PublishWorkerCount must be > 0.")
            .Validate(settings => settings.PublishBatchElementCount > 0, "Throughput:PublishBatchElementCount must be > 0.")
            .Validate(settings => settings.PublishBatchDelayMilliseconds > 0, "Throughput:PublishBatchDelayMilliseconds must be > 0.")
            .Validate(settings => settings.PublishBatchByteThreshold > 0, "Throughput:PublishBatchByteThreshold must be > 0.")
            .Validate(settings => settings.AckDeadlineSeconds > 0, "Throughput:AckDeadlineSeconds must be > 0.")
            .Validate(settings => settings.AckExtensionWindowSeconds > 0, "Throughput:AckExtensionWindowSeconds must be > 0.")
            .Validate(settings => settings.MaxTotalAckExtensionMinutes > 0, "Throughput:MaxTotalAckExtensionMinutes must be > 0.")
            .Validate(settings => settings.CompressionThresholdBytes >= 0, "Throughput:CompressionThresholdBytes must be >= 0.")
            .ValidateOnStart();
    }
}
