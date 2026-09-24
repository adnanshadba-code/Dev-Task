using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Application.Mappings;
using DevTeam.Services.Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Services.Identity.Application.Commands.Permissions
{
    public class UpdatePermissionCommand : IRequest<PermissionDto?>
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }

    public class UpdatePermissionHandler
    : IRequestHandler<UpdatePermissionCommand, PermissionDto?>
    {
        private readonly IRepository<Permission> _repository;

        public UpdatePermissionHandler(
            IRepository<Permission> repository)
        {
            _repository = repository;
        }

        public async Task<PermissionDto?> Handle(
            UpdatePermissionCommand request,
            CancellationToken cancellationToken)
        {
            // Get permission

            var permission = await _repository
                .GetByIdAsync(request.Id);

            if (permission == null)
                return null;


            // Check duplicate

            var exists = await _repository
                .Query()
                .AnyAsync(
                    x =>
                        x.Name == request.Name &&
                        x.Id != request.Id,
                    cancellationToken);

            if (exists)
                throw new Exception(
                    "Permission already exists.");


            // Update

            request.UpdateFrom(permission);


            // Save

            _repository.Update(permission);

            await _repository.SaveChangesAsync();


            // Entity -> DTO

            return permission.ToDto();
        }
    }
}
