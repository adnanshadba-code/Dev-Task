using Shipments.BG.Consumer;

namespace ShipmentWorker;

public class Worker : BackgroundService
{
    //Inject IRabbitMQConsumer not RabbitMQConsumer =>(Interfaces) , (Abstraction)
    //receive message from rabbitMq(publisher)
    private readonly IRabbitMQConsumer _consumer;

    private readonly ILogger<Worker> _logger;


    //Contsructor => DI
    public Worker(IRabbitMQConsumer consumer,ILogger<Worker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }


    //ExcuteAsync => function in worker inheritance from  BackgroundService in Entityframework =>
   //start backgroung 
   //
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "======================================");

        _logger.LogInformation(
            "Shipment Worker started.");

        _logger.LogInformation("Starting RabbitMQ consumers...");

        _logger.LogInformation(
            "======================================");

        try
        {

            //StartAsync without await => because don't want one consume  waiting for the other to fineshed then start secouned consume. 

            //start to consume shipment
            var shipmentConsumer = _consumer.StartAsync("Shipment", stoppingToken);
            //satrt to consume shipmentEvents
            var shipmentEventConsumer = _consumer.StartAsync("ShipmentEvent", stoppingToken);
            // Shipment , ShipmentEvent => Queue configuration names define in appsetting.json


            
            await Task.WhenAll(shipmentConsumer, shipmentEventConsumer);//Task.WhenAll => 2 Quses working to gother and consumes 
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Shipment Worker stopping...");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(
                ex,"Shipment Worker stopped because of an unexpected error.");//RabbitMQ connection failed

            throw;
        }
    }
}