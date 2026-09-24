using DevTeam.Services.Shipments.Applications.Commands.Shipments;
using DevTeam.Services.Shipments.Applications.Messaging.RabbitMQMessages;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DevTeam.Application.Messaging.ShipmentMessages
{
    public class CreateShipmentMessage
    {
        public int Id { get; set; }

        public string TrackingNumber { get; set; } = string.Empty;

        public string Origin { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public decimal Weight { get; set; }

        public string Status { get; set; } = string.Empty;

        public int CarrierId { get; set; }
    }

    public class CreateShipmentMessageHandler : IRabbitMQMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateShipmentMessageHandler> _logger;

        public CreateShipmentMessageHandler(
            IMediator mediator,
            ILogger<CreateShipmentMessageHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public string MessageType => nameof(CreateShipmentMessage);

        public async Task HandleAsync(string json, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateShipmentMessageHandler started.");

            var message =
                JsonSerializer.Deserialize<RabbitMQMessage<CreateShipmentMessage>>(
                        json);

            if (message == null)
            {
                throw new Exception(
                    "Failed to deserialize CreateShipmentMessage.");
            }

            _logger.LogInformation(
                "CreateShipmentMessage deserialized successfully.");

            var command =
                new CreateShipmentCommand
                {
                    TrackingNumber =
                        message.Data.TrackingNumber,

                    Origin =
                        message.Data.Origin,

                    Destination =
                        message.Data.Destination,

                    Weight =
                        message.Data.Weight,

                    Status =
                        message.Data.Status,

                    CarrierId =
                        message.Data.CarrierId
                };

            _logger.LogInformation(
                "Sending CreateShipmentCommand to MediatR.");

            await _mediator.Send(command, cancellationToken);

            _logger.LogInformation(
                "CreateShipmentCommand processed successfully.");
        }
    }
}
