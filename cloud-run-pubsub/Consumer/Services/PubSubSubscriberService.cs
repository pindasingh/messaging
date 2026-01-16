using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Messages;

namespace Consumer.Services;

public class PubSubSubscriberService : BackgroundService
{
    private readonly ILogger<PubSubSubscriberService> _logger;
    private readonly PubSubSettings _settings;
    private SubscriberClient? _subscriber;

    public PubSubSubscriberService(
        ILogger<PubSubSubscriberService> logger,
        IOptions<PubSubSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriptionName = SubscriptionName.FromProjectSubscription(
            _settings.ProjectId,
            _settings.SubscriptionId);

        _subscriber = await SubscriberClient.CreateAsync(subscriptionName);

        _logger.LogInformation("Starting Pub/Sub subscriber for {Subscription}", _settings.SubscriptionId);

        await _subscriber.StartAsync((message, token) =>
        {
            try
            {
                var json = message.Data.ToStringUtf8();
                var sampleMessage = SampleMessage.FromJson(json);

                if (sampleMessage != null)
                {
                    _logger.LogInformation(
                        "Received message {MessageId}: {Content} (sent at {Timestamp})",
                        sampleMessage.Id,
                        sampleMessage.Content,
                        sampleMessage.Timestamp);
                }
                else
                {
                    _logger.LogWarning("Failed to deserialize message: {Json}", json);
                }

                return Task.FromResult(SubscriberClient.Reply.Ack);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                return Task.FromResult(SubscriberClient.Reply.Nack);
            }
        });

        // Wait for cancellation
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Expected when stopping
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Pub/Sub subscriber...");

        if (_subscriber != null)
        {
            await _subscriber.StopAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}
