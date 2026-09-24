
using DevTeam.Services.Shipments.Applications.Messaging.Queues;
using DevTeam.Services.Shipments.Applications.Messaging.RabbitMQMessages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace DevTeam.Services.Shipments.Applications.Messaging.Publisher;

public class RabbitMQPublisher : IRabbitMQPublisher
{
    private readonly RabbitMQOptions _options;
    private readonly ILogger<RabbitMQPublisher> _logger;

    public RabbitMQPublisher(
        IOptions<RabbitMQOptions> options,
        ILogger<RabbitMQPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task PublishAsync<T>(
        string queueName,
        T message)
    {
        var envelope = new RabbitMQMessage<T>
        {
            MessageType = typeof(T).Name,
            Data = message,
            Exception = null
        };

        await PublishEnvelopeAsync(
            queueName,
            envelope);
    }

    public async Task PublishEnvelopeAsync<T>(
        string queueName,
        RabbitMQMessage<T> message)
    {
        if (!_options.Queues.TryGetValue(
                queueName,
                out var queueOptions))
        {
            throw new Exception(
                $"RabbitMQ queue configuration not found: {queueName}");
        }

        _logger.LogInformation(
            "Publishing RabbitMQ message. Type: {MessageType}, Queue: {Queue}",
            message.MessageType,
            queueOptions.Queue);

        _logger.LogInformation(
            "Message Exception: {Exception}",
            message.Exception ?? "None");

        var json =
            JsonSerializer.Serialize(message);

        _logger.LogInformation(
            "Message JSON: {Json}",
            json);

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: queueOptions.Exchange,
            type: ExchangeType.Direct,
            durable: true);

        await channel.ExchangeDeclareAsync(
            exchange: queueOptions.DeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true);

        await channel.QueueDeclareAsync(
            queue: queueOptions.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: queueOptions.DeadLetterQueue,
            exchange: queueOptions.DeadLetterExchange,
            routingKey: queueOptions.DeadLetterRoutingKey);

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
            arguments: arguments);

        await channel.QueueBindAsync(
            queue: queueOptions.Queue,
            exchange: queueOptions.Exchange,
            routingKey: queueOptions.RoutingKey);

        var body =
            Encoding.UTF8.GetBytes(json);

        var properties =
            new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

        await channel.BasicPublishAsync(
            exchange: queueOptions.Exchange,
            routingKey: queueOptions.RoutingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);

        _logger.LogInformation(
            "RabbitMQ message published successfully. Type: {MessageType}, Queue: {Queue}",
            message.MessageType,
            queueOptions.Queue);
    }

    public async Task PublishToDeadLetterAsync<T>(
        string queueName,
        RabbitMQMessage<T> message)
    {
        if (!_options.Queues.TryGetValue(
                queueName,
                out var queueOptions))
        {
            throw new Exception(
                $"RabbitMQ queue configuration not found: {queueName}");
        }

        var json =
            JsonSerializer.Serialize(message);

        _logger.LogInformation(
            "Publishing failed message to DLQ. Type: {MessageType}, DLQ: {DLQ}, Exception: {Exception}",
            message.MessageType,
            queueOptions.DeadLetterQueue,
            message.Exception);

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: queueOptions.DeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true);

        await channel.QueueDeclareAsync(
            queue: queueOptions.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: queueOptions.DeadLetterQueue,
            exchange: queueOptions.DeadLetterExchange,
            routingKey: queueOptions.DeadLetterRoutingKey);

        var body =
            Encoding.UTF8.GetBytes(json);

        var properties =
            new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

        await channel.BasicPublishAsync(
            exchange: queueOptions.DeadLetterExchange,
            routingKey: queueOptions.DeadLetterRoutingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);

        _logger.LogWarning(
            "Failed message published to DLQ successfully. Type: {MessageType}, DLQ: {DLQ}",
            message.MessageType,
            queueOptions.DeadLetterQueue);
    }
}

