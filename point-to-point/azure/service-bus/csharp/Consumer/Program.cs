using Consumer.Handlers;
using Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceBusQueueSettings(builder.Configuration);

builder.Services.AddHostedService<OrderCreatedHandler>();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();
