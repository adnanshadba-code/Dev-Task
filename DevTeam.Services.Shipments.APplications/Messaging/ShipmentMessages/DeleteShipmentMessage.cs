using DevTeam.Services.Shipments.Applications.Commands.ShipmentEvents;
using DevTeam.Services.Shipments.Applications.Messaging.RabbitMQMessages;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DevTeam.Application.Messaging.ShipmentMessages
{
    public class DeleteShipmentMessage
    {
        public int Id { get; set; }
    }
    public class DeleteShipmentMessageHandler : IRabbitMQMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DeleteShipmentMessageHandler> _logger;

        public DeleteShipmentMessageHandler(
            IMediator mediator,
            ILogger<DeleteShipmentMessageHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public string MessageType =>
            nameof(DeleteShipmentMessage);

        public async Task HandleAsync(
            string json,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "DeleteShipmentMessageHandler started.");

            var message =
                JsonSerializer.Deserialize<
                    RabbitMQMessage<DeleteShipmentMessage>>(
                        json);

            if (message == null)
            {
                throw new Exception(
                    "Failed to deserialize DeleteShipmentMessage.");
            }

            _logger.LogInformation(
                "DeleteShipmentMessage deserialized successfully.");

            var command = new DeleteShipmentEventCommand
            {
                Id = message.Data.Id
            };

            _logger.LogInformation(
                "Sending DeleteShipmentCommand for Shipment Id: {Id}",
                command.Id);

            await _mediator.Send(
                command,
                cancellationToken);

            _logger.LogInformation(
                "DeleteShipmentCommand processed successfully.");
        }
    }
}
