using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;
using Producer.Handlers;
using Producer.Models;
using Shared.Configuration;
using Shared.Messaging;

namespace Producer;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddPubSubSettings(builder.Configuration, requireTopic: true);
        builder.Services.AddThroughputSettings(builder.Configuration);

        builder.Services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<PubSubSettings>>().Value;
            var throughput = sp.GetRequiredService<IOptions<ThroughputSettings>>().Value;
            var topicName = TopicName.FromProjectTopic(settings.ProjectId, settings.TopicId);
            
            var publishers = new List<PublisherClient>();
            var publisherBuilder = PublisherSettingsFactory.Create(throughput);
            
            for (var index = 0; index < throughput.PublishWorkerCount; index++)
            {
                // Multiple publishers increase parallelism when a single client is CPU-bound.
                publisherBuilder.TopicName = topicName;
                publishers.Add(publisherBuilder.Build());
            }

            return (IReadOnlyList<PublisherClient>)publishers;
        });

        builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ThroughputSettings>>().Value);

        builder.Services.AddScoped<OrderCreatedPublisher>();
        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.MapHealthChecks("/health");

        app.MapPost("/publish", (PublishMessageRequest request, OrderCreatedPublisher publisher) =>
            publisher.Publish(request));

        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopping.Register(() =>
        {
            // Shutdown all publisher clients to flush outstanding batches.
            foreach (var publisher in app.Services.GetRequiredService<IReadOnlyList<PublisherClient>>())
            {
                publisher.ShutdownAsync(TimeSpan.FromSeconds(10)).Wait();
            }
        });

        app.Run();
    }
}
