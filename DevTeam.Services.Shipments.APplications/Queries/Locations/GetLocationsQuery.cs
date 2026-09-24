using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Application.Queries.Locations
{
    public class GetLocationsQuery : IRequest<List<LocationDto>>  // Location طلب  لاسترجاع   , Request to retrive all location  // 
    {

    }

    public class GetLocationsHandler : IRequestHandler<GetLocationsQuery, List<LocationDto>>  // تنفيذ الطلب 
    {

        private readonly IRepository<Location> _repository;

        public GetLocationsHandler(IRepository<Location> repository)
        {
            _repository = repository;
        }

        public async Task<List<LocationDto>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
        {
            //Query => Iquerable<Location> ==> excuate on DB layer 
            //ASNotracking => i need to read data without Change Tracker
            //ToListAsync => excuate query using database when using .Query()
            //await => asynch
            //locations.Select(x => x.ToDto()).ToList(); 
            //ToList => work with memory not database 
            var locations = await _repository.Query().AsNoTracking().ToListAsync(cancellationToken);
            //location => list<Location> on memory application
            return locations.Select(x => x.ToDto()).ToList();
        }


    }
}
