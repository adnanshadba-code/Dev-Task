using DevTeam.Services.Shipments.Applications.Commands.ShipmentEvents;
using DevTeam.Services.Shipments.Applications.Messaging.RabbitMQMessages;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DevTeam.Application.Messaging.ShipmentEventMessage
{
    public class UpdateShipmentEventMessage
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }
        public int LocationId { get; set; }
        public string Status { get; set; }


        //public string EventCode { get; set; } = string.Empty;

        //public string Description { get; set; } = string.Empty;
    }

    public class UpdateShipmentEventMessageHandler : IRabbitMQMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateShipmentEventMessageHandler> _logger;

        public UpdateShipmentEventMessageHandler(
            IMediator mediator,
            ILogger<UpdateShipmentEventMessageHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public string MessageType =>
            nameof(UpdateShipmentEventMessage);

        public async Task HandleAsync(
            string json,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "UpdateShipmentEventMessageHandler started.");

            var message =
                JsonSerializer.Deserialize<
                    RabbitMQMessage<UpdateShipmentEventMessage>>(
                        json);

            if (message == null)
            {
                throw new Exception(
                    "Failed to deserialize UpdateShipmentEventMessage.");
            }

            _logger.LogInformation(
                "UpdateShipmentEventMessage deserialized successfully.");

            var command =
                new UpdateShipmentEventCommand
                {
                    Id =
                        message.Data.Id,

                    ShipmentId =
                        message.Data.ShipmentId,
                    LocationId = message.Data.LocationId,
                    Status = message.Data.Status,
                    //UpdateBy=message.Data.

                    //EventCode =
                    //    message.Data.EventCode,

                    //Description =
                    //    message.Data.Description
                };

            _logger.LogInformation(
                "Sending UpdateShipmentEventCommand for Id: {Id}",
                command.Id);

            await _mediator.Send(
                command,
                cancellationToken);

            _logger.LogInformation(
                "UpdateShipmentEventCommand processed successfully.");
        }
    }
}
