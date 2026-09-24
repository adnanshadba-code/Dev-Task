using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Services.Shipments.Applications.Commands.Locations
{

    public class UpdateLocationCommand : IRequest<LocationDto?>
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string? UpdateBy { get; set; }
    }

    public class UpdateLocationHandler // this handler ecxute to this Request (UpdateLocationCommand), this response(LocationDto) هو اللي بنفذ الطلب
     : IRequestHandler<UpdateLocationCommand, LocationDto?>
    {
        private readonly IRepository<Location> _repository;
        //private readonly IUnitOfWork _unitOfWork;

        public UpdateLocationHandler(IRepository<Location> repository)

        {
            _repository = repository;
            //_unitOfWork = unitOfWork;
        }

        public async Task<LocationDto?> Handle(
            UpdateLocationCommand request,
            CancellationToken cancellationToken)
        {
            // Get existing Entity
            var location = await _repository.Query().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (location == null)
                return null;

            // Command → Existing Entity
            request.UpdateFrom(location);

            // Update
            _repository.Update(location);

            // Save
            await _repository.SaveChangesAsync();

            // Entity → DTO
            return location.ToDto();
        }
    }
}
