using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Interfaces;
using SharedKernel.Repositories;

namespace DevTeam.Services.Identity.Application.Commands.Users
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string? CreateBy { get; set; }

        public List<int> PermissionIds { get; set; } = new();
    }


    public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IRepository<User> _repository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<UserPermission> _userPermissionRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ICache _cache;

        public CreateUserHandler(IRepository<User> repository, IRepository<Permission> permissionRepository, IRepository<UserPermission> userPermissionRepository, IPasswordHasher<User> passwordHasher, ICache cache)
        {
            _repository = repository;
            _permissionRepository = permissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _passwordHasher = passwordHasher;
            _cache = cache;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Check username
            var usernameExists = await _repository.Query().AnyAsync(x => x.Username == request.Username, cancellationToken);

            if (usernameExists)
                throw new Exception("Username already exists.");

            // 2. Command -> Entity
            var user = request.ToEntity();

            // 3. Password Hashing
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            // 4. Add User
            await _repository.AddAsync(user);

            await _repository.SaveChangesAsync();

            // 5. Add Permissions

            foreach (var permissionId in request.PermissionIds)
            {
                var permission =
                    await _permissionRepository
                        .GetByIdAsync(permissionId);

                if (permission == null)
                {
                    throw new Exception(
                        $"Permission with Id {permissionId} not found.");
                }

                var userPermission = new UserPermission
                {
                    UserId = user.Id,
                    PermissionId = permissionId
                };

                await _userPermissionRepository.AddAsync(userPermission);
            }


            // 6. Save UserPermissions

            await _userPermissionRepository.SaveChangesAsync();


            // 7. Remove Users Cache

            await _cache.RemoveAsync("users:all", cancellationToken);

            // 7. Entity -> DTO

            return user.ToDto();
        }
    }
}