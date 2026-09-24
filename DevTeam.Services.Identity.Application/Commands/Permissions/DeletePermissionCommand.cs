using DevTeam.Services.Identity.Domain.Entities;
using MediatR;
using SharedKernel.Repositories;

namespace DevTeam.Services.Identity.Application.Commands.Permissions
{
    public class DeletePermissionCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeletePermissionHandler
    : IRequestHandler<DeletePermissionCommand, bool>
    {
        private readonly IRepository<Permission> _repository;

        public DeletePermissionHandler(
            IRepository<Permission> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(
            DeletePermissionCommand request,
            CancellationToken cancellationToken)
        {
            var permission = await _repository
                .GetByIdAsync(request.Id);

            if (permission == null)
                return false;

            _repository.Delete(permission);

            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
