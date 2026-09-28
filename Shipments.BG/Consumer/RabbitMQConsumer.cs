//using DevTeam.Application.Messaging;
//using DevTeam.Services.Shipments.Applications.Messaging.Publisher;
//using DevTeam.Services.Shipments.Applications.Messaging.Queues;
//using DevTeam.Services.Shipments.Applications.Messaging.RabbitMQMessages;
//using Microsoft.Extensions.Options;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using Shipments.BG.Consumer;
//using System.Collections;
//using System.Text;
//using System.Text.Json;

//namespace ShipmentWorker;

//public class RabbitMQConsumer : IRabbitMQConsumer
//{
//    private readonly RabbitMQOptions _options;//define in configration => appsetting.json
//    private readonly IServiceScopeFactory _scopeFactory; // to work new scope for each message
//    private readonly ILogger<RabbitMQConsumer> _logger;
//    private readonly IRabbitMQPublisher _publisher;

//    public RabbitMQConsumer(
//        IOptions<RabbitMQOptions> options,
//        IServiceScopeFactory scopeFactory,
//        ILogger<RabbitMQConsumer> logger,
//        IRabbitMQPublisher publisher)
//    {
//        _options = options.Value;
//        _scopeFactory = scopeFactory;
//        _logger = logger;
//        _publisher = publisher;
//    }

//    public async Task StartAsync(string queueName,CancellationToken cancellationToken)
//    {
//        _logger.LogInformation("Starting RabbitMQ consumer for queue: {QueueName}", queueName);


//        //search queue configration in appsettings.json (shipment , ShipmentEvents)
//        if (!_options.Queues.TryGetValue(queueName, out var queueOptions))
//        {
//            throw new Exception(
//                $"RabbitMQ queue configuration not found: {queueName}");
//        }


//        //build new object to get correct rabbitMq 
//        //Ex: HostName = localhost
//             //Username = admin
//             //Password = admin
//             //VirtualHost = /
//             //Port = 5672
//        var factory = new ConnectionFactory
//        {
//            HostName = _options.Host,
//            Port = _options.Port,
//            UserName = _options.Username,
//            Password = _options.Password,
//            VirtualHost = _options.VirtualHost
//        };

//        //ConnectionAsync => create connection with rabbitMQServer (Ex:docker)
//        await using var connection =await factory.CreateConnectionAsync(cancellationToken);

//        //ChannelAsync=> root in Connection to excute rabbitMq opertion
//        //Ex:Exchange
//            //Queue
//            //Bind
//           //Consume
//           //ACK
//           //NACK
//        await using var channel =await connection.CreateChannelAsync(cancellationToken: cancellationToken);


//        //Exchange => receive message from queue publisher because publisher not send message to rabbitMq direct 
//        //publisher send to Exchange =>  and Exchange determine any queue  use 
//        //=type: ExchangeType.Direct => routing for queue ,

//        await channel.ExchangeDeclareAsync(
//            exchange: queueOptions.Exchange,
//            type: ExchangeType.Direct,//root to queue 
//            durable: true,//to be Exchange persistent and not disappearance when colse connection
//            cancellationToken: cancellationToken);


//        //Create Echange  to DLQ when faild message 
//        await channel.ExchangeDeclareAsync(
//            exchange: queueOptions.DeadLetterExchange,
//            type: ExchangeType.Direct,//root to queue 
//            durable: true,
//            cancellationToken: cancellationToken);


//        //Craete DLQ to faild message 
//        await channel.QueueDeclareAsync(
//            queue: queueOptions.DeadLetterQueue,
//            durable: true,//if restart queue => Queue contain ths same messages  
//            exclusive: false,//queue foun  a lot of connection
//            autoDelete: false,//not delete queue when consume message 
//            cancellationToken: cancellationToken);


//        //when faild message
//        //QueueBindAsync => to link between DLQ and DLX
//        await channel.QueueBindAsync(
//            queue: queueOptions.DeadLetterQueue, //DLQ 
//            exchange: queueOptions.DeadLetterExchange, //DLX => root to detarmin faild message DLQ
//            routingKey: queueOptions.DeadLetterRoutingKey,// root to detarmin queue 
//            cancellationToken: cancellationToken);

//        //
//        var arguments = new Dictionary<string, object?>
//        {
//            ["x-dead-letter-exchange"] = queueOptions.DeadLetterExchange,

//            ["x-dead-letter-routing-key"] = queueOptions.DeadLetterRoutingKey
//        };



//        //create normal queue when succsess 
//        //        "Queue": "devteam.shipments",

//        await channel.QueueDeclareAsync(
//            queue: queueOptions.Queue,
//            durable: true,
//            exclusive: false,
//            autoDelete: false,
//            arguments: arguments,
//            cancellationToken: cancellationToken);


//        await channel.QueueBindAsync(
//            queue: queueOptions.Queue,
//            exchange: queueOptions.Exchange,
//            routingKey: queueOptions.RoutingKey,
//            cancellationToken: cancellationToken);


//        //craete consumer when message arrives queue 
//        var consumer =new AsyncEventingBasicConsumer(channel);

//        //Event handler => excute code when recive message in queue 
//        consumer.ReceivedAsync += async (sender, args) =>
//        {
//            //sender => object 
//            //args => message information 

//            string json = string.Empty;//


//            RabbitMQMessage<JsonElement>? envelope = null;

//            try
//            {
//                _logger.LogInformation("RabbitMQ message received from queue: {Queue}",queueOptions.Queue);

//                //body => recived body in bytes  and trnsfer to string 
//                json =Encoding.UTF8.GetString(args.Body.ToArray());

//                _logger.LogInformation("Received message: {Json}",json);

//                //Deserialize => trnsfer json to object 
//                envelope = JsonSerializer.Deserialize<RabbitMQMessage<JsonElement>>(json);

//                if (envelope == null)
//                {
//                    throw new Exception("Failed to deserialize RabbitMQ message.");
//                }

//                _logger.LogInformation("MessageType: {MessageType}, Exception: {Exception}",
//                    envelope.MessageType,
//                    envelope.Exception ?? "None");

//                //create scope to contain handler , repo
//                using var scope =_scopeFactory.CreateScope();

//                //create handler 
//                var handler =scope.ServiceProvider.GetServices<IRabbitMQMessageHandler>()
//                        .FirstOrDefault(
//                            x => x.MessageType ==
//                                 envelope.MessageType);

//                if (handler == null)
//                {
//                    throw new Exception(
//                        $"No handler found for message type: " +
//                        $"{envelope.MessageType}");
//                }

//                _logger.LogInformation(
//                    "Executing handler: {HandlerName}",
//                    handler.GetType().Name);

//                //excute business logic 
//                await handler.HandleAsync(json,cancellationToken);


//                //ACK = Acknowledgement => if meesage succsess not returned message to queue 
//                await channel.BasicAckAsync(
//                    deliveryTag: args.DeliveryTag,//id messages => for each message have specific delivrey tag
//                    multiple: false);




//                _logger.LogInformation(
//                    "Message processed successfully and ACK sent. MessageType: {MessageType}",
//                    envelope.MessageType);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(
//                    ex,
//                    "RabbitMQ message processing failed. MessageType: {MessageType}",
//                    envelope?.MessageType ?? "Unknown");

//                if (envelope == null)
//                {
//                    _logger.LogWarning("Envelope is null. Sending original message to DLQ.");

//                    //NACK = Negative Acknowledgement => when message faild
//                    await channel.BasicNackAsync(
//                        deliveryTag: args.DeliveryTag,
//                        multiple: false,
//                        requeue: false//not return message to same queue becaus  
//                        );

//                    return;
//                }

//                envelope.Exception = ex.Message;

//                try
//                {
//                    await _publisher.PublishToDeadLetterAsync("Shipment",envelope);

//                    await channel.BasicAckAsync(
//                        deliveryTag: args.DeliveryTag,
//                        multiple: false);

//                    _logger.LogWarning(
//                        "Failed message published to Shipment DLQ and original message ACKed. Exception: {Exception}",
//                        envelope.Exception);
//                }
//                catch (Exception publishException)
//                {
//                    _logger.LogError(
//                        publishException,
//                        "Failed to publish modified message to Shipment DLQ.");

//                    await channel.BasicNackAsync(
//                        deliveryTag: args.DeliveryTag,
//                        multiple: false,
//                        requeue: false);

//                    _logger.LogWarning(
//                        "Original message NACK sent. Original message will be moved to DLQ.");
//                }
//            }
//        };

//        //statrt consume 
//        await channel.BasicConsumeAsync(
//            queue: queueOptions.Queue,
//            autoAck: false,
//            consumer: consumer);

//        _logger.LogInformation(
//            "RabbitMQ consumer started and listening on queue: {Queue}",
//            queueOptions.Queue);

//        try
//        {
//            //consumer not retrive to worker 
//            await Task.Delay(Timeout.Infinite,cancellationToken);
//        }
//        catch (OperationCanceledException)
//        {
//            _logger.LogInformation(
//                "RabbitMQ consumer stopped for queue: {Queue}",
//                queueOptions.Queue);
//        }
//    }
//}

using DevTeam.Application.Messaging;
using DevTeam.Services.Shipments.Applications.Messaging.Publisher;
using DevTeam.Services.Shipments.Applications.Messaging.Queues;
using DevTeam.Services.Shipments.Applications.Messaging.RabbitMQMessages;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Shipments.BG.Consumer;

using System.Text;
using System.Text.Json;

namespace ShipmentWorker;

public class RabbitMQConsumer : IRabbitMQConsumer
{
    // RabbitMQ configuration loaded from appsettings.json
    private readonly RabbitMQOptions _options;

    // Creates a new DI scope for every received message.
    // This is important because handlers/repositories/DbContext
    // are usually registered as Scoped services.
    private readonly IServiceScopeFactory _scopeFactory;

    // Used to write logs to the console/application logs.
    private readonly ILogger<RabbitMQConsumer> _logger;

    // Used when we need to publish a failed message to the DLQ.
    private readonly IRabbitMQPublisher _publisher;


    public RabbitMQConsumer(
        IOptions<RabbitMQOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMQConsumer> logger,
        IRabbitMQPublisher publisher)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _publisher = publisher;
    }


    public async Task StartAsync(
        string queueName,
        CancellationToken cancellationToken)
    {
        // ============================================================
        // 1. Get Queue Configuration
        // ============================================================

        _logger.LogInformation(
            "Starting RabbitMQ consumer for queue: {QueueName}",
            queueName);


        // Find the requested queue configuration from appsettings.json.
        //
        // Example:
        // queueName = "Shipment"
        //
        // _options.Queues["Shipment"]
        //
        // This gives us:
        // Exchange
        // Queue
        // RoutingKey
        // DeadLetterExchange
        // DeadLetterQueue
        // DeadLetterRoutingKey

        if (!_options.Queues.TryGetValue(
                queueName,
                out var queueOptions))
        {
            throw new Exception(
                $"RabbitMQ queue configuration not found: {queueName}");
        }


        // ============================================================
        // 2. Create RabbitMQ Connection
        // ============================================================

        // ConnectionFactory contains the information required
        // to connect to the RabbitMQ server.
        //
        // Example:
        // Host     = localhost
        // Port     = 5672
        // Username = admin
        // Password = admin
        // VHost    = /

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };


        // Create a connection with RabbitMQ.
        await using var connection =
            await factory.CreateConnectionAsync(
                cancellationToken);


        // ============================================================
        // 3. Create RabbitMQ Channel
        // ============================================================

        // A Channel is a logical communication path inside
        // the RabbitMQ connection.
        //
        // We use the channel to:
        //
        // - Declare Exchange
        // - Declare Queue
        // - Bind Queue to Exchange
        // - Consume messages
        // - ACK messages
        // - NACK messages

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);


        // ============================================================
        // 4. Create Main Exchange
        // ============================================================

        // Publisher sends the message to the Exchange,
        // not directly to the Queue.
        //
        // The Direct Exchange uses the RoutingKey
        // to determine which Queue should receive the message.

        await channel.ExchangeDeclareAsync(
            exchange: queueOptions.Exchange,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken);


        // ============================================================
        // 5. Create Dead Letter Exchange (DLX)
        // ============================================================

        // The DLX receives messages that could not be processed
        // successfully.

        await channel.ExchangeDeclareAsync(
            exchange: queueOptions.DeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken);


        // ============================================================
        // 6. Create Dead Letter Queue (DLQ)
        // ============================================================

        // The DLQ stores messages that failed processing.
        //
        // Example:
        //
        // Normal Queue
        //      ↓
        //   Processing
        //      ↓
        //    FAILED
        //      ↓
        //     DLX
        //      ↓
        //     DLQ

        await channel.QueueDeclareAsync(
            queue: queueOptions.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);


        // ============================================================
        // 7. Bind DLQ to DLX
        // ============================================================

        // This tells RabbitMQ:
        //
        // "Messages arriving at the DLX with this RoutingKey
        // should be delivered to this DLQ."

        await channel.QueueBindAsync(
            queue: queueOptions.DeadLetterQueue,
            exchange: queueOptions.DeadLetterExchange,
            routingKey: queueOptions.DeadLetterRoutingKey,
            cancellationToken: cancellationToken);


        // ============================================================
        // 8. Configure Dead Letter Behavior for Main Queue
        // ============================================================

        // These arguments tell RabbitMQ:
        //
        // If a message in the MAIN queue becomes a Dead Letter,
        // send it to the following DLX using the following RoutingKey.

        var deadLetterArguments =
            new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] =
                    queueOptions.DeadLetterExchange,

                ["x-dead-letter-routing-key"] =
                    queueOptions.DeadLetterRoutingKey
            };


        // ============================================================
        // 9. Create Main Queue
        // ============================================================

        await channel.QueueDeclareAsync(
            queue: queueOptions.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: deadLetterArguments,
            cancellationToken: cancellationToken);


        // ============================================================
        // 10. Bind Main Queue to Main Exchange
        // ============================================================

        // This creates:
        //
        // Main Exchange
        //      ↓
        // RoutingKey
        //      ↓
        // Main Queue

        await channel.QueueBindAsync(
            queue: queueOptions.Queue,
            exchange: queueOptions.Exchange,
            routingKey: queueOptions.RoutingKey,
            cancellationToken: cancellationToken);


        // ============================================================
        // 11. Create RabbitMQ Consumer
        // ============================================================

        // AsyncEventingBasicConsumer listens for messages
        // arriving in the queue.

        var consumer =
            new AsyncEventingBasicConsumer(channel);


        // ============================================================
        // 12. Handle Received Messages
        // ============================================================

        // This code runs every time RabbitMQ delivers
        // a message to this consumer.

        consumer.ReceivedAsync += async (sender, args) =>
        {
            string json = string.Empty;

            RabbitMQMessage<JsonElement>? envelope = null;

            try
            {
                // ----------------------------------------------------
                // Receive Message
                // ----------------------------------------------------

                _logger.LogInformation(
                    "RabbitMQ message received from queue: {Queue}",
                    queueOptions.Queue);


                // RabbitMQ message body is received as bytes.
                // Convert bytes to UTF-8 string.

                json = Encoding.UTF8.GetString(
                    args.Body.ToArray());


                _logger.LogInformation(
                    "Received message: {Json}",
                    json);


                // ----------------------------------------------------
                // Deserialize Message
                // ----------------------------------------------------

                // Convert JSON string into our RabbitMQ envelope.

                envelope =
                    JsonSerializer.Deserialize<
                        RabbitMQMessage<JsonElement>>(json);


                if (envelope == null)
                {
                    throw new Exception(
                        "Failed to deserialize RabbitMQ message.");
                }


                _logger.LogInformation(
                    "MessageType: {MessageType}, Exception: {Exception}",
                    envelope.MessageType,
                    envelope.Exception ?? "None");


                // ----------------------------------------------------
                // Create DI Scope
                // ----------------------------------------------------

                // Create a new scope for this message.
                //
                // The scope contains:
                // Handler
                // Repository
                // DbContext
                // UnitOfWork
                // etc.

                using var scope =
                    _scopeFactory.CreateScope();


                // ----------------------------------------------------
                // Find Message Handler
                // ----------------------------------------------------

                // Get all registered message handlers
                // and find the handler that supports this MessageType.

                var handler =
                    scope.ServiceProvider
                        .GetServices<IRabbitMQMessageHandler>()
                        .FirstOrDefault(
                            x => x.MessageType ==
                                 envelope.MessageType);


                if (handler == null)
                {
                    throw new Exception(
                        $"No handler found for message type: " +
                        $"{envelope.MessageType}");
                }


                _logger.LogInformation(
                    "Executing handler: {HandlerName}",
                    handler.GetType().Name);


                // ----------------------------------------------------
                // Execute Business Logic
                // ----------------------------------------------------

                // The Handler is responsible for the actual work.
                //
                // Example:
                //
                // Deserialize Data
                //       ↓
                // Create Shipment
                //       ↓
                // Repository
                //       ↓
                // Database

                await handler.HandleAsync(
                    json,
                    cancellationToken);


                // ----------------------------------------------------
                // ACK Message
                // ----------------------------------------------------

                // Handler completed successfully.
                //
                // Tell RabbitMQ:
                // "This message was processed successfully.
                //  Do not deliver it again."

                await channel.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false);


                _logger.LogInformation(
                    "Message processed successfully and ACK sent. " +
                    "MessageType: {MessageType}",
                    envelope.MessageType);
            }
            catch (Exception ex)
            {
                // ====================================================
                // Message Processing Failed
                // ====================================================

                _logger.LogError(
                    ex,
                    "RabbitMQ message processing failed. " +
                    "MessageType: {MessageType}",
                    envelope?.MessageType ?? "Unknown");


                // ----------------------------------------------------
                // Case 1: Envelope could not be created
                // ----------------------------------------------------

                // We cannot modify the message because
                // deserialization failed.

                if (envelope == null)
                {
                    _logger.LogWarning(
                        "Envelope is null. " +
                        "Sending original message to DLQ.");


                    // NACK tells RabbitMQ that processing failed.
                    //
                    // requeue:false means:
                    // "Do not put the message back into
                    // the same queue."

                    await channel.BasicNackAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false,
                        requeue: false);

                    return;
                }


                // ----------------------------------------------------
                // Case 2: Envelope exists
                // ----------------------------------------------------

                // Store the actual error inside the message
                // before sending it to the DLQ.

                envelope.Exception = ex.Message;


                try
                {
                    // Publish the failed message manually
                    // to the Dead Letter Queue.

                    await _publisher.PublishToDeadLetterAsync(queueName,envelope);


                    // The failed message was successfully copied
                    // to the DLQ.
                    //
                    // Now ACK the original message so RabbitMQ
                    // does not deliver it again.

                    await channel.BasicAckAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false);


                    _logger.LogWarning(
                    "Failed message published to {QueueName} DLQ " +
                    "and original message ACKed. " +
                    "Exception: {Exception}",
                     queueName,
                     envelope.Exception);
                }
                catch (Exception publishException)
                {
                    // ------------------------------------------------
                    // DLQ Publishing Failed
                    // ------------------------------------------------

                    _logger.LogError(
                        publishException,
                        "Failed to publish modified message " +
                        "to Shipment DLQ.");


                    // Since our manual DLQ publishing failed,
                    // let RabbitMQ handle the Dead Letter operation.

                    await channel.BasicNackAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false,
                        requeue: false);


                    _logger.LogWarning(
                        "Original message NACK sent. " +
                        "Original message will be moved to DLQ.");
                }
            }
        };


        // ============================================================
        // 13. Start Consuming Messages
        // ============================================================

        // autoAck:false means:
        //
        // RabbitMQ will NOT automatically acknowledge messages.
        //
        // We will ACK only after the Handler succeeds.

        await channel.BasicConsumeAsync(
            queue: queueOptions.Queue,
            autoAck: false,
            consumer: consumer);


        _logger.LogInformation(
            "RabbitMQ consumer started and listening on queue: {Queue}",
            queueOptions.Queue);


        // ============================================================
        // 14. Keep Consumer Alive
        // ============================================================

        // The consumer must remain alive and listen for messages.
        //
        // It will wait indefinitely until the application
        // sends a cancellation request.

        try
        {
            await Task.Delay(Timeout.Infinite,cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("RabbitMQ consumer stopped for queue: {Queue}",queueOptions.Queue);
        }
    }
}
