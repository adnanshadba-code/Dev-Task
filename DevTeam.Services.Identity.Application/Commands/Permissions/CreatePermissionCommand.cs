using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Application.Mappings;
using DevTeam.Services.Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Services.Identity.Application.Commands.Permissions
{
    public class CreatePermissionCommand : IRequest<PermissionDto>
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }

    public class CreatePermissionHandler: IRequestHandler<CreatePermissionCommand, PermissionDto>
    {
        private readonly IRepository<Permission> _repository;

        public CreatePermissionHandler(IRepository<Permission> repository)
        {
            _repository = repository;
        }

        public async Task<PermissionDto> Handle(CreatePermissionCommand request,CancellationToken cancellationToken)
        {
            // Check duplicate

            var exists = await _repository.Query().AnyAsync(x => x.Name == request.Name,cancellationToken);

            if (exists)
                throw new Exception("Permission already exists.");

            // Command -> Entity

            var permission = request.ToEntity();

            // Add

            await _repository.AddAsync(permission);

            await _repository.SaveChangesAsync();

            // Entity -> DTO

            return permission.ToDto();
        }
    }
}
