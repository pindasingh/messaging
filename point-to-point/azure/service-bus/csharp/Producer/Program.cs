using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Producer.Handlers;
using Producer.Models;
using Shared.Configuration;

namespace Producer;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddServiceBusQueueSettings(builder.Configuration);

        builder.Services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ServiceBusQueueSettings>>().Value;
            return new ServiceBusClient(settings.ConnectionString);
        });

        builder.Services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ServiceBusQueueSettings>>().Value;
            var client = sp.GetRequiredService<ServiceBusClient>();
            return client.CreateSender(settings.QueueName);
        });

        builder.Services.AddScoped<OrderCreatedPublisher>();
        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.MapHealthChecks("/health");

        app.MapPost("/publish", (PublishMessageRequest request, OrderCreatedPublisher publisher) =>
            publisher.Publish(request));

        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopping.Register(() =>
        {
            var client = app.Services.GetRequiredService<ServiceBusClient>();
            client.DisposeAsync().AsTask().GetAwaiter().GetResult();
        });

        app.Run();
    }
}
