using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Producer.Models;
using Shared.Events;

namespace Producer.Handlers;

public class OrderCreatedPublisher
{
    private readonly EventHubProducerClient _producer;
    private readonly ILogger<OrderCreatedPublisher> _logger;

    public OrderCreatedPublisher(EventHubProducerClient producer, ILogger<OrderCreatedPublisher> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public async Task<IResult> Publish(PublishMessageRequest request)
    {
        var payload = new OrderCreatedV1 { Content = request.Message };
        using var batch = await _producer.CreateBatchAsync();
        var eventData = new EventData(payload.ToJson());

        if (!batch.TryAdd(eventData))
        {
            return Results.BadRequest("Message too large for the Event Hubs batch.");
        }

        await _producer.SendAsync(batch);

        _logger.LogInformation("Published Event Hub message with content: {Content}", request.Message);

        return Results.Ok(new PublishMessageResponse(eventData.MessageId ?? string.Empty));
    }
}
