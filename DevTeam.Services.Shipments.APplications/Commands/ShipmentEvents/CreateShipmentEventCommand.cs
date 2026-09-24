using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Services.Shipments.Applications.Commands.ShipmentEvents
{

    //ShipmentEventDto return to Client (Response)
    public class CreateShipmentEventCommand : IRequest<ShipmentEventDto>
    {
        public int ShipmentId { get; set; }
        public string Status { get; set; }
        public string CreateBy { get; set; }
        public int LocationId { get; set; }
    }

    public class CreateShipmentEventHandler : IRequestHandler<CreateShipmentEventCommand, ShipmentEventDto>
    {
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IRepository<Location> _locationRepository;
        private readonly IRepository<ShipmentEvent> _repository;
        //private readonly IShipmentEventRabbitMQMessageHandler _rabbitMQPublisher;

        public CreateShipmentEventHandler(IRepository<ShipmentEvent> repository, IRepository<Shipment> shipmentRepository, IRepository<Location> locationRepository)
        {
            _repository = repository;
            _shipmentRepository = shipmentRepository;
            _locationRepository = locationRepository;
        }



        public async Task<ShipmentEventDto> Handle(CreateShipmentEventCommand request, CancellationToken cancellationToken)
        {
            var shipmentFound = await _shipmentRepository.Query().FirstOrDefaultAsync(x => x.Id == request.ShipmentId, cancellationToken);
            if (shipmentFound == null)
                throw new Exception("Shipment does not exists.");

            var locationExists = await _locationRepository.Query().FirstOrDefaultAsync(x => x.Id == request.LocationId, cancellationToken);
            if (locationExists == null)
                throw new Exception("Location does not exists.");


            var shipmentEvent = request.ToEntity();

            await _repository.AddAsync(shipmentEvent);

            await _repository.SaveChangesAsync();

            //// 3. Create RabbitMQ Message
            //var message = new ShipmentEventMessage
            //{
            //    Id = shipmentEvent.Id,
            //    ShipmentId = shipmentEvent.ShipmentId,
            //    Status=shipmentEvent.Status
            //    //EventCode = shipmentEvent.,
            //    //Description = shipmentEvent.Description,
            //    //CreateAt = shipmentEvent.CreatedAt
            //};

            // //Publish to RabbitMQ
            //await _rabbitMQPublisher.PublishAsync(message);

            // Return API response
            return shipmentEvent.ToDto();

        }
    }
}
