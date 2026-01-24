using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Events;

namespace Consumer.Handlers;

public class OrderCreatedHandler : BackgroundService
{
    private readonly ILogger<OrderCreatedHandler> _logger;
    private readonly ServiceBusTopicSettings _settings;
    private ServiceBusProcessor? _processor;
    private ServiceBusClient? _client;

    public OrderCreatedHandler(
        ILogger<OrderCreatedHandler> logger,
        IOptions<ServiceBusTopicSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client = new ServiceBusClient(_settings.ConnectionString);
        _processor = _client.CreateProcessor(_settings.TopicName, _settings.SubscriptionName);

        _processor.ProcessMessageAsync += HandleMessageAsync;
        _processor.ProcessErrorAsync += HandleErrorAsync;

        _logger.LogInformation(
            "Starting Service Bus topic processor for {Topic}/{Subscription}",
            _settings.TopicName,
            _settings.SubscriptionName);

        await _processor.StartProcessingAsync(stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Service Bus topic processor cancellation requested.");
        }
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var json = args.Message.Body.ToString();
            var orderCreated = OrderCreatedV1.FromJson(json)
                ?? throw new InvalidOperationException($"Deserialized message was null: {json}");

            _logger.LogInformation(
                "Received topic message {MessageId}: {Content} (sent at {Timestamp})",
                args.Message.MessageId,
                orderCreated.Content,
                orderCreated.Timestamp);

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Service Bus topic message");
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task HandleErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Service Bus topic processor error");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Service Bus topic processor");

        if (_processor != null)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        if (_client != null)
        {
            await _client.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
