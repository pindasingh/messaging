using Microsoft.Extensions.DependencyInjection;
using Shared.Configuration;

namespace Consumer.Push.Messaging;

public static class PushProcessingServiceCollectionExtensions
{
    public static IServiceCollection AddPushProcessingQueue(this IServiceCollection services)
    {
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ThroughputSettings>>().Value);
        services.AddSingleton<ProcessingQueue>();
        services.AddHostedService<BackgroundProcessor>();

        return services;
    }
}
