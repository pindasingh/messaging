using Shared.Events;

namespace Consumer.Push.Handlers;

public class OrderCreatedHandler
{
    private readonly ILogger<OrderCreatedHandler> _logger;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger)
    {
        _logger = logger;
    }

    public IResult Handle(PubSubPushRequest request)
    {
        try
        {
            var data = request.Message.DecodeData();
            var orderCreated = OrderCreatedV1.FromJson(data)
                ?? throw new InvalidOperationException($"Deserialized message was null: {data}");

            _logger.LogInformation(
                "Received message {MessageId}: {Content} (sent at {Timestamp})",
                orderCreated.Id,
                orderCreated.Content,
                orderCreated.Timestamp);

            return Results.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            return Results.StatusCode(500);
        }
    }
}

public class PubSubPushRequest
{
    public PubSubMessageData Message { get; set; } = new();
    public string Subscription { get; set; } = string.Empty;
}

public class PubSubMessageData
{
    public string Data { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public DateTime PublishTime { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();

    public string DecodeData()
    {
        var bytes = Convert.FromBase64String(Data);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}
