using Consumer.Pull.Handlers;
using Shared.Configuration;

namespace Consumer.Pull;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddPubSubSettings(builder.Configuration, requireSubscription: true);
        builder.Services.AddThroughputSettings(builder.Configuration);

        builder.Services.AddHostedService<StreamingPullOrderCreatedHandler>();
        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.MapHealthChecks("/health");

        app.Run();
    }
}
