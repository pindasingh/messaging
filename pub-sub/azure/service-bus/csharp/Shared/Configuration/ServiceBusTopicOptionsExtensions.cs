using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Configuration;

public static class ServiceBusTopicOptionsExtensions
{
    public static OptionsBuilder<ServiceBusTopicSettings> AddServiceBusTopicSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddOptions<ServiceBusTopicSettings>()
            .Bind(configuration.GetSection(ServiceBusTopicSettings.SectionName))
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.ConnectionString), "ServiceBus:ConnectionString is required.")
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.TopicName), "ServiceBus:TopicName is required.")
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.SubscriptionName), "ServiceBus:SubscriptionName is required.")
            .ValidateOnStart();
    }
}
