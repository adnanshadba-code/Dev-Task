namespace DevTeam.Services.Shipments.Applications.Messaging.Queues
{
    public class RabbitMQOptions//Configration model => how to connect with rabbitmq and contain data for calls
    {

        //
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public Dictionary<string, RabbitMQQueueOptions> Queues { get; set; } = new();// based on defintion  queues in appsettings.json 
    }
    public class RabbitMQQueueOptions
    {
        public string Exchange { get; set; } = string.Empty;
        public string Queue { get; set; } = string.Empty;
        public string RoutingKey { get; set; } = string.Empty;

        public string DeadLetterExchange { get; set; } = string.Empty;
        public string DeadLetterQueue { get; set; } = string.Empty;
        public string DeadLetterRoutingKey { get; set; } = string.Empty;
    }
}
