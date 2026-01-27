using Shared.Messaging;
using PushMessaging = Consumer.Push.Messaging;

namespace Consumer.Push.Handlers;

public class HttpPushOrderCreatedHandler
{
    private readonly ILogger<HttpPushOrderCreatedHandler> _logger;
    private readonly PushMessaging.ProcessingQueue _queue;

    public HttpPushOrderCreatedHandler(ILogger<HttpPushOrderCreatedHandler> logger, PushMessaging.ProcessingQueue queue)
    {
        _logger = logger;
        _queue = queue;
    }

    public async Task<IResult> HandleAsync(PushMessaging.PubSubPushRequest request, CancellationToken cancellationToken)
    {
        // Ack immediately to keep push delivery fast; do the heavy lifting in the background.
        // Background processing avoids holding up Pub/Sub push delivery retries.
        await _queue.EnqueueAsync(token =>
        {
            try
            {
                var data = PushMessaging.PushMessageDecoder.Decode(request.Message);
                var orderCreated = OrderCreatedV1.FromJson(data)
                    ?? throw new InvalidOperationException($"Deserialized message was null: {data}");

                _logger.LogInformation(
                    "Processed push message {MessageId}: {Content} (sent at {Timestamp})",
                    request.Message.MessageId,
                    orderCreated.Content,
                    orderCreated.Timestamp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing push message {MessageId}", request.Message.MessageId);
            }

            return Task.CompletedTask;
        }, cancellationToken);

        return Results.Ok();
    }
}
