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
}
