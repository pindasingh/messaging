using Google.Cloud.Tasks.V2;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Producer.Models;
using Shared.Configuration;
using Shared.Events;
using CloudTasksHttpMethod = Google.Cloud.Tasks.V2.HttpMethod;
using CloudTasksHttpRequest = Google.Cloud.Tasks.V2.HttpRequest;

namespace Producer.Handlers;

public class OrderCreatedPublisher
{
    private const string TaskPath = "/tasks";
    private readonly CloudTasksClient _client;
    private readonly TasksSettings _settings;
    private readonly ILogger<OrderCreatedPublisher> _logger;

    public OrderCreatedPublisher(
        CloudTasksClient client,
        IOptions<TasksSettings> settings,
        ILogger<OrderCreatedPublisher> logger)
    {
        _client = client;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<IResult> Publish(PublishMessageRequest request)
    {
        var payload = new OrderCreatedV1 { Content = request.Message };
        var queueName = QueueName.FromProjectLocationQueue(
            _settings.ProjectId,
            _settings.LocationId,
            _settings.QueueId);

        var httpRequest = new CloudTasksHttpRequest
        {
            HttpMethod = CloudTasksHttpMethod.Post,
            Url = BuildTargetUrl(),
            Headers = { { "Content-Type", "application/json" } },
            Body = ByteString.CopyFromUtf8(payload.ToJson())
        };

        var task = new Google.Cloud.Tasks.V2.Task { HttpRequest = httpRequest };
        var response = await _client.CreateTaskAsync(queueName, task);

        _logger.LogInformation("Enqueued task {TaskName} with content: {Content}", response.Name, request.Message);

        return Results.Ok(new PublishMessageResponse(response.Name));
    }

    private string BuildTargetUrl()
    {
        var builder = new UriBuilder(_settings.TargetUrl);
        if (string.IsNullOrWhiteSpace(builder.Path) || builder.Path == "/")
        {
            builder.Path = TaskPath;
        }
        else if (!builder.Path.EndsWith(TaskPath, StringComparison.OrdinalIgnoreCase))
        {
            builder.Path = $"{builder.Path.TrimEnd('/')}{TaskPath}";
        }

        return builder.Uri.ToString();
    }
}
