

using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Application.Mappings;
using DevTeam.Services.Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Services.Identity.Application.Queries.Permissions
{
    public class GetPermissionByIdQuery
    : IRequest<PermissionDto?>
    {
        public int Id { get; set; }
    }

    public class GetPermissionByIdHandler
    : IRequestHandler<GetPermissionByIdQuery, PermissionDto?>
    {
        private readonly IRepository<Permission> _repository;

        public GetPermissionByIdHandler(
            IRepository<Permission> repository)
        {
            _repository = repository;
        }

        public async Task<PermissionDto?> Handle(
            GetPermissionByIdQuery request,
            CancellationToken cancellationToken)
        {
            var permission = await _repository
                .Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    cancellationToken);

            if (permission == null)
                return null;

            return permission.ToDto();
        }
    }
}
