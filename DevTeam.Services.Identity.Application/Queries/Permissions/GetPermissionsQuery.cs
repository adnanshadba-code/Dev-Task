using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Application.Mappings;
using DevTeam.Services.Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Services.Identity.Application.Queries.Permissions
{
    public class GetPermissionsQuery : IRequest<List<PermissionDto>>
    {
    }

    public class GetPermissionsHandler : IRequestHandler<GetPermissionsQuery, List<PermissionDto>>
    {
        private readonly IRepository<Permission> _repository;

        public GetPermissionsHandler(
            IRepository<Permission> repository)
        {
            _repository = repository;
        }

        public async Task<List<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _repository.Query().AsNoTracking().ToListAsync(cancellationToken);

            return permissions.Select(x => x.ToDto()).ToList();
        }
    }
}
