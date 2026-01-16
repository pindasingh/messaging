using Consumer.Services;
using Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<PubSubSettings>(
    builder.Configuration.GetSection(PubSubSettings.SectionName));

// Register background service
builder.Services.AddHostedService<PubSubSubscriberService>();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Health endpoint
app.MapHealthChecks("/health");

app.Run();
