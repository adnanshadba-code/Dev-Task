using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Application.Queries.ShipmentsEvents
{
    public class GetShipmentEventsByShipmentQuery : IRequest<List<ShipmentEventDto>>
    {
        public int ShipmentId { get; set; }
    }

    public class GetShipmentEventsByShipmentHandler
    : IRequestHandler<
        GetShipmentEventsByShipmentQuery,
        List<ShipmentEventDto>>
    {
        private readonly IRepository<ShipmentEvent> _repository;
        private readonly IRepository<Shipment> _shipmentRepository;

        public GetShipmentEventsByShipmentHandler(
            IRepository<ShipmentEvent> repository,
            IRepository<Shipment> shipmentRepository)
        {
            _repository = repository;
            _shipmentRepository = shipmentRepository;
        }

        public async Task<List<ShipmentEventDto>> Handle(
            GetShipmentEventsByShipmentQuery request,
            CancellationToken cancellationToken)
        {
            var shipmentExists = await _shipmentRepository
                .Query()
                .AnyAsync(
                    s => s.Id == request.ShipmentId,
                    cancellationToken);

            if (!shipmentExists)
                throw new Exception("Shipment does not exist.");

            var events = await _repository
                .Query()
                .Include(e => e.Location)
                .AsNoTracking()
                .Where(e => e.ShipmentId == request.ShipmentId)
                .ToListAsync(cancellationToken);

            return events
                .Select(e => e.ToDto())
                .ToList();
        }
    }

}
