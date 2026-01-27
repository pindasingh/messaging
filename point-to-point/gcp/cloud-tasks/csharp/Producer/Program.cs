using Google.Cloud.Tasks.V2;
using Microsoft.Extensions.Options;
using Producer.Handlers;
using Producer.Models;
using Shared.Configuration;

namespace Producer;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddTasksSettings(builder.Configuration);

        builder.Services.AddSingleton(CloudTasksClient.Create());

        builder.Services.AddScoped<OrderCreatedPublisher>();
        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.MapHealthChecks("/health");

        app.MapPost("/publish", (PublishMessageRequest request, OrderCreatedPublisher publisher) =>
            publisher.Publish(request));

        app.Run();
    }
}
