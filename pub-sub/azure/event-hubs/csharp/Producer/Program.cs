using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Options;
using Producer.Handlers;
using Producer.Models;
using Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEventHubsSettings(builder.Configuration);

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<EventHubsSettings>>().Value;
    return new EventHubProducerClient(settings.ConnectionString, settings.EventHubName);
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
    var producer = app.Services.GetRequiredService<EventHubProducerClient>();
    producer.CloseAsync().GetAwaiter().GetResult();
});

app.Run();
