using DevTeam.Services.Shipments.Applications.Commands.ShipmentEvents;
using DevTeam.Services.Shipments.Applications.Messaging.RabbitMQMessages;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DevTeam.Application.Messaging.ShipmentEventMessage
{
    public class DeleteShipmentEventMessage
    {
        public int Id { get; set; }
    }

    public class DeleteShipmentEventMessageHandler
    : IRabbitMQMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DeleteShipmentEventMessageHandler> _logger;

        public DeleteShipmentEventMessageHandler(IMediator mediator, ILogger<DeleteShipmentEventMessageHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public string MessageType =>
            nameof(DeleteShipmentEventMessage);

        public async Task HandleAsync(
            string json,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "DeleteShipmentEventMessageHandler started.");

            var message =
                JsonSerializer.Deserialize<
                    RabbitMQMessage<DeleteShipmentEventMessage>>(
                        json);

            if (message == null)
            {
                throw new Exception(
                    "Failed to deserialize DeleteShipmentEventMessage.");
            }

            _logger.LogInformation(
                "DeleteShipmentEventMessage deserialized successfully.");

            var command = new DeleteShipmentEventCommand
            {
                Id = message.Data.Id
            };


            _logger.LogInformation(
                "Sending DeleteShipmentEventCommand for Id: {Id}",
                command.Id);

            await _mediator.Send(
                command,
                cancellationToken);

            _logger.LogInformation(
                "DeleteShipmentEventCommand processed successfully.");
        }
    }
}
