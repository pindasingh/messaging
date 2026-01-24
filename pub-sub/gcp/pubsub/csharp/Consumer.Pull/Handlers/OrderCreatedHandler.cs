using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Events;

namespace Consumer.Pull.Handlers;

public class OrderCreatedHandler : BackgroundService
{
    private readonly ILogger<OrderCreatedHandler> _logger;
    private readonly PubSubSettings _settings;
    private SubscriberClient? _client;

    public OrderCreatedHandler(
        ILogger<OrderCreatedHandler> logger,
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

        _client = await SubscriberClient.CreateAsync(subscriptionName);

        _logger.LogInformation("Starting OrderCreated handler for {Subscription}", _settings.SubscriptionId);

        await _client.StartAsync((message, token) =>
        {
            try
            {
                var json = message.Data.ToStringUtf8();
                var orderCreated = OrderCreatedV1.FromJson(json)
                    ?? throw new InvalidOperationException($"Deserialized message was null: {json}");

                _logger.LogInformation(
                    "Received message {MessageId}: {Content} (sent at {Timestamp})",
                    orderCreated.Id,
                    orderCreated.Content,
                    orderCreated.Timestamp);

                return Task.FromResult(SubscriberClient.Reply.Ack);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                return Task.FromResult(SubscriberClient.Reply.Nack);
            }
        });

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("OrderCreated handler cancellation requested.");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping OrderCreated handler");

        if (_client != null)
        {
            await _client.StopAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}
