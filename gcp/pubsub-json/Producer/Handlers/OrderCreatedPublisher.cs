using Google.Cloud.PubSub.V1;
using Producer.Models;
using Shared.Events;

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
        var message = new OrderCreatedV1 { Content = request.Message };
        var messageId = await _publisher.PublishAsync(message.ToJson());

        _logger.LogInformation("Published message {MessageId} with content: {Content}", messageId, request.Message);

        return Results.Ok(new PublishMessageResponse(messageId));
    }
}
