namespace DevTeam.Services.Shipments.Applications.Messaging.RabbitMQMessages
{

    //
    public class RabbitMQMessage<T>
    {
        public string MessageType { get; set; } = string.Empty;
        public T Data { get; set; } = default!;
        public string? Exception { get; set; }
    }
}
