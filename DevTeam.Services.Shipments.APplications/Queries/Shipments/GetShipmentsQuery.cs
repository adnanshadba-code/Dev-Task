using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Application.Queries.GetShipments;

public class GetShipmentsQuery : IRequest<List<ShipmentDto>>
{
}

public class GetShipmentsHandler
    : IRequestHandler<GetShipmentsQuery, List<ShipmentDto>>
{
    private readonly IRepository<Shipment> _repository;

    public GetShipmentsHandler(
        IRepository<Shipment> repository)
    {
        _repository = repository;
    }

    public async Task<List<ShipmentDto>> Handle(
        GetShipmentsQuery request,
        CancellationToken cancellationToken)
    {
        var shipments = await _repository
            .Query()
            .Include(s => s.Carrier)
            .Include(s => s.ShipmentEvents)
                .ThenInclude(e => e.Location)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return shipments
            .Select(x => x.ToDto())
            .ToList();
    }
}