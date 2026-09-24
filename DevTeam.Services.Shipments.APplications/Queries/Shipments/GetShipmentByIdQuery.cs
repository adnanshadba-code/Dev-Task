using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Application.Queries.GetShipmentById;

public class GetShipmentByIdQuery : IRequest<ShipmentDto?>
{
    public int Id { get; set; }
}


public class GetShipmentByIdHandler
    : IRequestHandler<GetShipmentByIdQuery, ShipmentDto?>
{
    private readonly IRepository<Shipment> _repository;

    public GetShipmentByIdHandler(
        IRepository<Shipment> repository)
    {
        _repository = repository;
    }

    public async Task<ShipmentDto?> Handle(
        GetShipmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var shipment = await _repository
            .Query()
            .Include(s => s.Carrier)
            .Include(s => s.ShipmentEvents)
                .ThenInclude(e => e.Location)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                s => s.Id == request.Id,
                cancellationToken);

        return shipment.ToDto();
    }
}