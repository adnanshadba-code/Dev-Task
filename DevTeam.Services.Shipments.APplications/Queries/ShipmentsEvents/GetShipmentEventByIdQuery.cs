using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Application.Queries.ShipmentsEvents
{
    public class GetShipmentEventByIdQuery
      : IRequest<ShipmentEventDto?>
    {
        public int Id { get; set; }
    }

    public class GetShipmentEventByIdHandler
    : IRequestHandler<
        GetShipmentEventByIdQuery,
        ShipmentEventDto?>
    {
        private readonly IRepository<ShipmentEvent> _repository;

        public GetShipmentEventByIdHandler(
            IRepository<ShipmentEvent> repository)
        {
            _repository = repository;
        }

        public async Task<ShipmentEventDto?> Handle(
            GetShipmentEventByIdQuery request,
            CancellationToken cancellationToken)
        {
            var shipmentEvent = await _repository
                .Query()
                .Include(e => e.Location).Include(x => x.Shipment)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    e => e.Id == request.Id,
                    cancellationToken);

            return shipmentEvent?.ToDto();
        }
    }
}
