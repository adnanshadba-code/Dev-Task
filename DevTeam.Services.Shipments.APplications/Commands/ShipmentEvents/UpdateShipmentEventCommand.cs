using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Services.Shipments.Applications.Commands.ShipmentEvents
{
    public class UpdateShipmentEventCommand : IRequest<ShipmentEventDto>
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public int ShipmentId { get; set; }

        public int LocationId { get; set; }

        public string? UpdateBy { get; set; }
    }

    public class UpdateShipmentEventHandler : IRequestHandler<UpdateShipmentEventCommand, ShipmentEventDto>
    {
        private readonly IRepository<ShipmentEvent> _repository;
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IRepository<Location> _locationRepository;

        public UpdateShipmentEventHandler(
            IRepository<ShipmentEvent> repository,
            IRepository<Shipment> shipmentRepository,
            IRepository<Location> locationRepository)
        {
            _repository = repository;
            _shipmentRepository = shipmentRepository;
            _locationRepository = locationRepository;
        }
        public async Task<ShipmentEventDto> Handle(UpdateShipmentEventCommand request, CancellationToken cancellationToken)
        {

            // validat shipmentEvent
            var shipmentEvent = await _repository.Query().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (shipmentEvent == null)
            {
                return null;
            }


            //existence validation (true or flase) use AnyAsync
            //validat shipment (existence validation)
            //FirstOrDefault more deatils 
            //FirstOrDefault => return Entity (shipment.Id , shipment.TrackingNumber , ..... ) and you can return null 

            //when i validate  shipment any thing the best soluation using (anyAsync)  not  FirstOrDefault  because you dont need to use entitty  
            var shipment = await _shipmentRepository.Query().FirstOrDefaultAsync(x => x.Id == request.ShipmentId, cancellationToken);
            if (shipment == null)
            {
                return null;
            }

            //validate location
            //best soluation because not not need data from entity  (true or false)
            var location = await _locationRepository.Query().AnyAsync(x => x.Id == request.LocationId, cancellationToken);
            if (!location)
            {
                throw new Exception("Location does not exists");
            }

            //methods with IQueryable
            //Where()
            //Select()
            //OrderBy()
            //Include()
            //FirstOrDefaultAsync()
            //AnyAsync()
            //CountAsync()
            //ToListAsync()

            request.UpdateFrom(shipmentEvent);

            _repository.Update(shipmentEvent);

            await _repository.SaveChangesAsync();

            return shipmentEvent.ToDto();

        }
    }


}
