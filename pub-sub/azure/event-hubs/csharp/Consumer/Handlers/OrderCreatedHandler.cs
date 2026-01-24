using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Events;

namespace Consumer.Handlers;

public class OrderCreatedHandler : BackgroundService
{
    private readonly ILogger<OrderCreatedHandler> _logger;
    private readonly EventHubsSettings _settings;
    private EventHubConsumerClient? _consumer;

    public OrderCreatedHandler(
        ILogger<OrderCreatedHandler> logger,
        IOptions<EventHubsSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer = new EventHubConsumerClient(
            _settings.ConsumerGroup,
            _settings.ConnectionString,
            _settings.EventHubName);

        _logger.LogInformation("Starting Event Hubs consumer group {ConsumerGroup}", _settings.ConsumerGroup);

        await foreach (var partitionEvent in _consumer.ReadEventsAsync(stoppingToken))
        {
            try
            {
                var json = Encoding.UTF8.GetString(partitionEvent.Data.EventBody.ToArray());
                var orderCreated = OrderCreatedV1.FromJson(json)
                    ?? throw new InvalidOperationException($"Deserialized message was null: {json}");

                _logger.LogInformation(
                    "Received Event Hub event {EventId}: {Content} (sent at {Timestamp})",
                    partitionEvent.Data.MessageId ?? string.Empty,
                    orderCreated.Content,
                    orderCreated.Timestamp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Event Hubs message");
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Event Hubs consumer");

        if (_consumer != null)
        {
            await _consumer.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}
