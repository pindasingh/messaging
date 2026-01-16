using Consumer.Push.Handlers;
using Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPubSubSettings(builder.Configuration, requireSubscription: true);

builder.Services.AddScoped<OrderCreatedHandler>();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapPost("/push", (PubSubPushRequest request, OrderCreatedHandler handler) =>
    handler.Handle(request));

app.Run();
