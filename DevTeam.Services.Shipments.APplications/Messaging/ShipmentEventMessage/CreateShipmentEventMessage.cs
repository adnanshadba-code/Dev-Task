using DevTeam.Services.Shipments.Applications.Commands.ShipmentEvents;
using DevTeam.Services.Shipments.Applications.Messaging.RabbitMQMessages;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DevTeam.Application.Messaging.ShipmentEventMessage
{
    public class CreateShipmentEventMessage
    {
        public int ShipmentId { get; set; }
        public string Status { get; set; }
        public int LocationId { get; set; }

        //public string EventCode { get; set; } = string.Empty;

        //public string Description { get; set; } = string.Empty;
    }

    //    //CreateShipmentMessageHandler
    //          │
    //          │ يحول
    //          ▼
    //CreateShipmentCommand

    public class CreateShipmentEventMessageHandler : IRabbitMQMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateShipmentEventMessageHandler> _logger;

        public CreateShipmentEventMessageHandler(
            IMediator mediator,
            ILogger<CreateShipmentEventMessageHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public string MessageType => nameof(CreateShipmentEventMessage);

        public async Task HandleAsync(
            string json,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "CreateShipmentEventMessageHandler started.");

            var message =
                JsonSerializer.Deserialize<
                    RabbitMQMessage<CreateShipmentEventMessage>>(
                        json);

            if (message == null)
            {
                throw new Exception(
                    "Failed to deserialize CreateShipmentEventMessage.");
            }

            _logger.LogInformation(
                "CreateShipmentEventMessage deserialized successfully.");

            var command =
                new CreateShipmentEventCommand
                {
                    ShipmentId =
                        message.Data.ShipmentId,
                    LocationId = message.Data.LocationId,
                    Status = message.Data.Status,
                    // CreateBy=message.Data.

                    //EventCode =
                    //    message.Data.EventCode,

                    //Description =
                    //    message.Data.Description
                };

            _logger.LogInformation(
                "Sending CreateShipmentEventCommand.");

            await _mediator.Send(
                command,
                cancellationToken);

            _logger.LogInformation(
                "CreateShipmentEventCommand processed successfully.");
        }
    }
}

