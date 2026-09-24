namespace Shipments.BG.Consumer
{
    public interface IRabbitMQConsumer
    {
        Task StartAsync(string queueName, CancellationToken cancellationToken);
    }
}
