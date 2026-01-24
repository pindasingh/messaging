using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Configuration;

public static class ServiceBusQueueOptionsExtensions
{
    public static OptionsBuilder<ServiceBusQueueSettings> AddServiceBusQueueSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddOptions<ServiceBusQueueSettings>()
            .Bind(configuration.GetSection(ServiceBusQueueSettings.SectionName))
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.ConnectionString), "ServiceBus:ConnectionString is required.")
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.QueueName), "ServiceBus:QueueName is required.")
            .ValidateOnStart();
    }
}
