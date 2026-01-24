using Consumer.Handlers;
using Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEventHubsSettings(builder.Configuration, requireConsumerGroup: true);

builder.Services.AddHostedService<OrderCreatedHandler>();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();
