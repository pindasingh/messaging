using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Configuration;

public static class EventHubsOptionsExtensions
{
    public static OptionsBuilder<EventHubsSettings> AddEventHubsSettings(
        this IServiceCollection services,
        IConfiguration configuration,
        bool requireConsumerGroup = false)
    {
        return services.AddOptions<EventHubsSettings>()
            .Bind(configuration.GetSection(EventHubsSettings.SectionName))
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.ConnectionString), "EventHubs:ConnectionString is required.")
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.EventHubName), "EventHubs:EventHubName is required.")
            .Validate(settings => !requireConsumerGroup || !string.IsNullOrWhiteSpace(settings.ConsumerGroup), "EventHubs:ConsumerGroup is required.")
            .ValidateOnStart();
    }
}
