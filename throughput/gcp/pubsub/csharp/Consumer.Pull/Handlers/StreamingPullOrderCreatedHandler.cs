using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Messaging;

namespace Consumer.Pull.Handlers;

public class StreamingPullOrderCreatedHandler : BackgroundService
{
    private readonly ILogger<StreamingPullOrderCreatedHandler> _logger;
    private readonly PubSubSettings _settings;
    private readonly ThroughputSettings _throughputSettings;
    private SubscriberClient? _client;

    public StreamingPullOrderCreatedHandler(
        ILogger<StreamingPullOrderCreatedHandler> logger,
        IOptions<PubSubSettings> settings,
        IOptions<ThroughputSettings> throughputSettings)
    {
        _logger = logger;
        _settings = settings.Value;
        _throughputSettings = throughputSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriptionName = SubscriptionName.FromProjectSubscription(
            _settings.ProjectId,
            _settings.SubscriptionId);

        var clientBuilder = SubscriberSettingsFactory.Create(_throughputSettings);
        clientBuilder.SubscriptionName = subscriptionName;
        _client = await clientBuilder.BuildAsync(stoppingToken);

        // Semaphore enforces bounded concurrency.
        var handler = new SemaphoreSlim(_throughputSettings.MaxConcurrentMessages);

        _logger.LogInformation("Starting high-throughput pull consumer for {Subscription}", _settings.SubscriptionId);

        await _client.StartAsync(async (message, token) =>
        {
            await handler.WaitAsync(token);

            try
            {
                var payload = message.Data.ToByteArray();
                var data = DecodePayload(payload, message.Attributes);
                var orderCreated = OrderCreatedV1.FromJson(data)
                    ?? throw new InvalidOperationException($"Deserialized message was null: {data}");

                _logger.LogInformation(
                    "Received message {MessageId}: {Content} (sent at {Timestamp})",
                    message.MessageId,
                    orderCreated.Content,
                    orderCreated.Timestamp);

                return SubscriberClient.Reply.Ack;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pull message");
                return SubscriberClient.Reply.Nack;
            }
            finally
            {
                handler.Release();
            }
        });

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pull consumer cancellation requested.");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping pull consumer");

        if (_client != null)
        {
            await _client.StopAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }

    private static string DecodePayload(byte[] payload, IReadOnlyDictionary<string, string> attributes)
    {
        if (attributes.TryGetValue(CompressionHelper.ContentEncodingAttribute, out var encoding)
            && string.Equals(encoding, CompressionHelper.GzipEncoding, StringComparison.OrdinalIgnoreCase))
        {
            return CompressionHelper.Decompress(payload);
        }

        return System.Text.Encoding.UTF8.GetString(payload);
    }
}
