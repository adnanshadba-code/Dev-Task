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
    private readonly RabbitMQOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMQConsumer> _logger;
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
        _logger.LogInformation(
            "Starting RabbitMQ consumer for queue: {QueueName}", queueName);

        if (!_options.Queues.TryGetValue(queueName, out var queueOptions))
        {
            throw new Exception(
                $"RabbitMQ queue configuration not found: {queueName}");
        }

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        await using var connection =
            await factory.CreateConnectionAsync(
                cancellationToken);

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: queueOptions.Exchange,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: queueOptions.DeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queueOptions.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: queueOptions.DeadLetterQueue,
            exchange: queueOptions.DeadLetterExchange,
            routingKey: queueOptions.DeadLetterRoutingKey,
            cancellationToken: cancellationToken);

        var arguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] =
                queueOptions.DeadLetterExchange,

            ["x-dead-letter-routing-key"] =
                queueOptions.DeadLetterRoutingKey
        };

        await channel.QueueDeclareAsync(
            queue: queueOptions.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: queueOptions.Queue,
            exchange: queueOptions.Exchange,
            routingKey: queueOptions.RoutingKey,
            cancellationToken: cancellationToken);

        var consumer =
            new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            string json = string.Empty;

            RabbitMQMessage<JsonElement>? envelope = null;

            try
            {
                _logger.LogInformation(
                    "RabbitMQ message received from queue: {Queue}",
                    queueOptions.Queue);

                json =
                    Encoding.UTF8.GetString(
                        args.Body.ToArray());

                _logger.LogInformation(
                    "Received message: {Json}",
                    json);

                envelope =
                    JsonSerializer.Deserialize<
                        RabbitMQMessage<JsonElement>>(
                        json);

                if (envelope == null)
                {
                    throw new Exception(
                        "Failed to deserialize RabbitMQ message.");
                }

                _logger.LogInformation(
                    "MessageType: {MessageType}, Exception: {Exception}",
                    envelope.MessageType,
                    envelope.Exception ?? "None");

                using var scope =
                    _scopeFactory.CreateScope();

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

                await handler.HandleAsync(
                    json,
                    cancellationToken);

                await channel.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false);

                _logger.LogInformation(
                    "Message processed successfully and ACK sent. MessageType: {MessageType}",
                    envelope.MessageType);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "RabbitMQ message processing failed. MessageType: {MessageType}",
                    envelope?.MessageType ?? "Unknown");

                if (envelope == null)
                {
                    _logger.LogWarning(
                        "Envelope is null. Sending original message to DLQ.");

                    await channel.BasicNackAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false,
                        requeue: false);

                    return;
                }

                envelope.Exception = ex.Message;

                try
                {
                    await _publisher.PublishToDeadLetterAsync(
                        "Shipment",
                        envelope);

                    await channel.BasicAckAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false);

                    _logger.LogWarning(
                        "Failed message published to Shipment DLQ and original message ACKed. Exception: {Exception}",
                        envelope.Exception);
                }
                catch (Exception publishException)
                {
                    _logger.LogError(
                        publishException,
                        "Failed to publish modified message to Shipment DLQ.");

                    await channel.BasicNackAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false,
                        requeue: false);

                    _logger.LogWarning(
                        "Original message NACK sent. Original message will be moved to DLQ.");
                }
            }
        };

        await channel.BasicConsumeAsync(
            queue: queueOptions.Queue,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation(
            "RabbitMQ consumer started and listening on queue: {Queue}",
            queueOptions.Queue);

        try
        {
            await Task.Delay(
                Timeout.Infinite,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation(
                "RabbitMQ consumer stopped for queue: {Queue}",
                queueOptions.Queue);
        }
    }
}
