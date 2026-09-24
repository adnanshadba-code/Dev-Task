using DevTeam.Services.Identity.Domain.Entities;
using MediatR;
using SharedKernel.Interfaces;
using SharedKernel.Repositories;

namespace DevTeam.Services.Identity.Application.Commands.Users
{

    public class DeleteUserCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }


    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IRepository<User> _repository;
        private readonly ICache _cache;
        public DeleteUserHandler(IRepository<User> repository, ICache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id);

            if (user == null)
                return false;

            _repository.Delete(user);

            await _repository.SaveChangesAsync();

            await _cache.RemoveAsync($"user:{request.Id}", cancellationToken);

            await _cache.RemoveAsync("users:all", cancellationToken);

            return true;
        }

    }
}