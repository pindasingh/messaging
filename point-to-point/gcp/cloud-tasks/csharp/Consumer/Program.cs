using Consumer.Handlers;
using Shared.Configuration;
using Shared.Events;

namespace Consumer;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddTasksSettings(builder.Configuration);

        builder.Services.AddScoped<OrderCreatedHandler>();
        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.MapHealthChecks("/health");

        app.MapPost("/tasks", (HttpRequest request, OrderCreatedHandler handler, OrderCreatedV1 payload) =>
            handler.Handle(request, payload));

        app.Run();
    }
}
