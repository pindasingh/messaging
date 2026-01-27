using Consumer.Push.Handlers;
using Shared.Configuration;

namespace Consumer.Push;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddPubSubSettings(builder.Configuration, requireSubscription: true);

        builder.Services.AddScoped<OrderCreatedHandler>();
        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.MapHealthChecks("/health");

        app.MapPost("/push", (PubSubPushRequest request, OrderCreatedHandler handler) =>
            handler.Handle(request));

        app.Run();
    }
}
