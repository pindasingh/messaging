using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Producer.Models;
using PubSubProto.Messages;

namespace Producer.Handlers;

public class OrderCreatedPublisher
{
    private readonly PublisherClient _publisher;
    private readonly ILogger<OrderCreatedPublisher> _logger;

    public OrderCreatedPublisher(PublisherClient publisher, ILogger<OrderCreatedPublisher> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<IResult> Publish(PublishMessageRequest request)
    {
        var message = new OrderCreatedV1
        {
            Id = Guid.NewGuid().ToString(),
            Content = request.Message,
            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
        };

        var messageId = await _publisher.PublishAsync(message.ToByteString());

        _logger.LogInformation("Published message {MessageId} with content: {Content}", messageId, request.Message);

        return Results.Ok(new PublishMessageResponse(messageId));
    }
}
