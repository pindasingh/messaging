using Azure.Messaging.ServiceBus;
using Producer.Models;
using Shared.Events;

namespace Producer.Handlers;

public class OrderCreatedPublisher
{
    private readonly ServiceBusSender _sender;
    private readonly ILogger<OrderCreatedPublisher> _logger;

    public OrderCreatedPublisher(ServiceBusSender sender, ILogger<OrderCreatedPublisher> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task<IResult> Publish(PublishMessageRequest request)
    {
        var payload = new OrderCreatedV1 { Content = request.Message };
        var message = new ServiceBusMessage(payload.ToJson());

        await _sender.SendMessageAsync(message);

        _logger.LogInformation("Published Service Bus queue message with content: {Content}", request.Message);

        return Results.Ok(new PublishMessageResponse(message.MessageId));
    }
}
