using DevTeam.Services.Shipments.Applications.Messaging.RabbitMQMessages;

namespace DevTeam.Services.Shipments.Applications.Messaging.Publisher
{
    public interface IRabbitMQPublisher
    {
        Task PublishAsync<T>(string queueName, T message);
        Task PublishEnvelopeAsync<T>(string queueName, RabbitMQMessage<T> message);
        Task PublishToDeadLetterAsync<T>(string queueName, RabbitMQMessage<T> message);

    }
}
