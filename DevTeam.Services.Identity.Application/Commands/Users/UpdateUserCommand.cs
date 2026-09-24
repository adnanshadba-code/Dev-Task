using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Interfaces;
using SharedKernel.Repositories;

namespace DevTeam.Services.Identity.Application.Commands.Users
{
    public class UpdateUserCommand : IRequest<UserDto?>
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string? Password { get; set; }

        public string? UpdateBy { get; set; }
    }


    public class UpdateUserHandler
    : IRequestHandler<UpdateUserCommand, UserDto?>
    {
        private readonly IRepository<User> _repository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ICache _cache;
        public UpdateUserHandler(IRepository<User> repository, IPasswordHasher<User> passwordHasher, ICache cache)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _cache = cache;
        }

        public async Task<UserDto?> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Get User

            var user = await _repository
                .GetByIdAsync(request.Id);

            if (user == null)
                return null;


            // 2. Check Username

            var usernameExists = await _repository.Query().AnyAsync(x => x.Username == request.Username && x.Id != request.Id, cancellationToken);

            if (usernameExists)
                throw new Exception(
                    "Username already exists.");


            // 3. Update Entity

            request.UpdateFrom(user);


            // 4. Update Password if provided

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash =
                    _passwordHasher.HashPassword(
                        user,
                        request.Password);
            }


            // 5. Save

            _repository.Update(user);

            await _repository.SaveChangesAsync();

            // 6. Remove User Cache

            await _cache.RemoveAsync($"user:{request.Id}", cancellationToken);


            // 7. Remove All Users Cache

            await _cache.RemoveAsync("users:all", cancellationToken);


            // 6. Entity -> DTO

            return user.ToDto();
        }
    }
}
