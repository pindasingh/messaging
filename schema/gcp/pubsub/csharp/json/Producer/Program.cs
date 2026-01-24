using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;
using Producer.Handlers;
using Producer.Models;
using Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPubSubSettings(builder.Configuration, requireTopic: true);

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<PubSubSettings>>().Value;
    var topicName = TopicName.FromProjectTopic(settings.ProjectId, settings.TopicId);
    return PublisherClient.Create(topicName);
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
    var publisher = app.Services.GetRequiredService<PublisherClient>();
    publisher.ShutdownAsync(TimeSpan.FromSeconds(10)).Wait();
});

app.Run();
