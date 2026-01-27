using Consumer.Push.Handlers;
using Shared.Configuration;
using PushMessaging = Consumer.Push.Messaging;
using PushProcessingExtensions = Consumer.Push.Messaging.PushProcessingServiceCollectionExtensions;

namespace Consumer.Push;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddPubSubSettings(builder.Configuration, requireSubscription: true);
        builder.Services.AddThroughputSettings(builder.Configuration);

        PushProcessingExtensions.AddPushProcessingQueue(builder.Services);

        builder.Services.AddScoped<HttpPushOrderCreatedHandler>();
        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.MapHealthChecks("/health");

        app.MapPost("/push", (PushMessaging.PubSubPushRequest request, HttpPushOrderCreatedHandler handler, CancellationToken token) =>
            handler.HandleAsync(request, token));

        app.Run();
    }
}
