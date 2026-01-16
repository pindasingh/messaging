using Google.Cloud.PubSub.V1;
using Producer.Models;
using Shared.Configuration;
using Shared.Messages;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<PubSubSettings>(
    builder.Configuration.GetSection(PubSubSettings.SectionName));

// Register PublisherClient
builder.Services.AddSingleton(sp =>
{
    var settings = builder.Configuration
        .GetSection(PubSubSettings.SectionName)
        .Get<PubSubSettings>()!;

    var topicName = TopicName.FromProjectTopic(settings.ProjectId, settings.TopicId);
    return PublisherClient.Create(topicName);
});

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Health endpoint
app.MapHealthChecks("/health");

// Publish endpoint
app.MapPost("/publish", async (PublishMessageRequest request, PublisherClient publisher, ILogger<Program> logger) =>
{
    var message = new SampleMessage { Content = request.Message };
    var messageId = await publisher.PublishAsync(message.ToJson());

    logger.LogInformation("Published message {MessageId} with content: {Content}", messageId, request.Message);

    return Results.Ok(new PublishMessageResponse(messageId));
});

// Graceful shutdown
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    var publisher = app.Services.GetRequiredService<PublisherClient>();
    publisher.ShutdownAsync(TimeSpan.FromSeconds(10)).Wait();
});

app.Run();
