namespace DevTeam.Application.Messaging
{
    public interface IRabbitMQMessageHandler
    {
        string MessageType { get; }

        Task HandleAsync(string json, CancellationToken cancellationToken);
    }
}
