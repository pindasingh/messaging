using Microsoft.Extensions.Primitives;
using Shared.Events;

namespace Consumer.Handlers;

public class OrderCreatedHandler
{
    private const string TaskNameHeader = "X-CloudTasks-TaskName";
    private readonly ILogger<OrderCreatedHandler> _logger;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger)
    {
        _logger = logger;
    }

    public IResult Handle(HttpRequest request, OrderCreatedV1 payload)
    {
        try
        {
            if (payload == null)
            {
                return Results.BadRequest("Task payload is required.");
            }

            var taskName = TryGetHeader(request.Headers, TaskNameHeader);

            _logger.LogInformation(
                "Received task {TaskName}: {Content} (sent at {Timestamp})",
                taskName,
                payload.Content,
                payload.Timestamp);

            return Results.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing task");
            return Results.StatusCode(500);
        }
    }

    private static string TryGetHeader(IHeaderDictionary headers, string key)
    {
        return headers.TryGetValue(key, out StringValues value)
            ? value.ToString()
            : string.Empty;
    }
}
