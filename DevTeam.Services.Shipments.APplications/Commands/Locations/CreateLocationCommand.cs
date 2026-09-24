using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using SharedKernel.Repositories;


namespace DevTeam.Services.Shipments.Applications.Commands.Locations
{
    public class CreateLocationCommand : IRequest<LocationDto>
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
    }

    public class CreateLocationHandler : IRequestHandler<CreateLocationCommand, LocationDto>
    {
        private readonly IRepository<Location> _repository;

        public CreateLocationHandler(IRepository<Location> repository)
        {
            _repository = repository;
        }

        public async Task<LocationDto> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            //Command => Entity
            var location = request.ToEntity();

            //Add entity 
            await _repository.AddAsync(location);

            //Save to database
            await _repository.SaveChangesAsync();

            //Entity => DTO
            return LocationMapping.ToDto(location);
        }

    }

}
