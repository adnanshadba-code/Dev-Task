using Shipments.BG.Consumer;

namespace ShipmentWorker;

public class Worker : BackgroundService
{
    private readonly IRabbitMQConsumer _consumer;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IRabbitMQConsumer consumer,
        ILogger<Worker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "======================================");

        //_logger.LogInformation(
        //    "Shipment Worker started.");

        _logger.LogInformation("Starting RabbitMQ consumers...");

        _logger.LogInformation(
            "======================================");

        try
        {
            var shipmentConsumer = _consumer.StartAsync("Shipment", stoppingToken);

            var shipmentEventConsumer = _consumer.StartAsync("ShipmentEvent", stoppingToken);

            await Task.WhenAll(shipmentConsumer, shipmentEventConsumer);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Shipment Worker stopping...");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(
                ex,
                "Shipment Worker stopped because of an unexpected error.");

            throw;
        }
    }
}