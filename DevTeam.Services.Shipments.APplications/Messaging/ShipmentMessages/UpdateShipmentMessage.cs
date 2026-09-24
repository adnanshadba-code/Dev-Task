using DevTeam.Services.Shipments.Applications.Commands.Shipments;
using DevTeam.Services.Shipments.Applications.Messaging.RabbitMQMessages;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DevTeam.Application.Messaging.ShipmentMessages
{
    public class UpdateShipmentMessage
    {
        public int Id { get; set; }

        public string TrackingNumber { get; set; } = string.Empty;

        public string Origin { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public decimal Weight { get; set; }

        public string Status { get; set; } = string.Empty;

        public int CarrierId { get; set; }
    }

    public class UpdateShipmentMessageHandler
    : IRabbitMQMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateShipmentMessageHandler> _logger;

        public UpdateShipmentMessageHandler(
            IMediator mediator,
            ILogger<UpdateShipmentMessageHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public string MessageType =>
            nameof(UpdateShipmentMessage);

        public async Task HandleAsync(
            string json,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "UpdateShipmentMessageHandler started.");

            var message =
                JsonSerializer.Deserialize<
                    RabbitMQMessage<UpdateShipmentMessage>>(
                        json);

            if (message == null)
            {
                throw new Exception(
                    "Failed to deserialize UpdateShipmentMessage.");
            }

            _logger.LogInformation(
                "UpdateShipmentMessage deserialized successfully.");

            var command =
                new UpdateShipmentCommand
                {
                    Id =
                        message.Data.Id,

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
                "Sending UpdateShipmentCommand for Shipment Id: {Id}",
                command.Id);

            await _mediator.Send(
                command,
                cancellationToken);

            _logger.LogInformation(
                "UpdateShipmentCommand processed successfully.");
        }
    }
}
