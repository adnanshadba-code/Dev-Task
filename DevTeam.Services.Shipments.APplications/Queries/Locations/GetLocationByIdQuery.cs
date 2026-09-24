using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Application.Queries.Locations
{
    public class GetLocationByIdQuery : IRequest<LocationDto>
    {
        public int Id { get; set; }
    }
    public class GetLocationByIdHandler : IRequestHandler<GetLocationByIdQuery, LocationDto>
    {
        private readonly IRepository<Location> _repository;

        public GetLocationByIdHandler(IRepository<Location> repository)
        {
            _repository = repository;
        }

        public async Task<LocationDto> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
        {
            var location = await _repository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (location == null)
            {
                return null;
            }
            return LocationMapping.ToDto(location);
        }
    }

}
