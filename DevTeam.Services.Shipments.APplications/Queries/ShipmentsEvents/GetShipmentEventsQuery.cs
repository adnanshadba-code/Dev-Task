using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Application.Queries.ShipmentsEvents
{
    public class GetShipmentEventsQuery : IRequest<List<ShipmentEventDto>>
    {
    }
    public class GetShipmentEventsHandler : IRequestHandler<GetShipmentEventsQuery, List<ShipmentEventDto>>
    {

        private readonly IRepository<ShipmentEvent> _repository;

        public GetShipmentEventsHandler(IRepository<ShipmentEvent> repository)
        {
            _repository = repository;
        }

        public async Task<List<ShipmentEventDto>> Handle(GetShipmentEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _repository
                       .Query()
                       .Include(e => e.Location).Include(x => x.Shipment)
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);

            return events
                .Select(e => e.ToDto())
                .ToList();
        }
    }
}
