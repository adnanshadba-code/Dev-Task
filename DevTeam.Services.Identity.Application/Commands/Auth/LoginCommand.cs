//using DevTeam.DTOs;
//using DevTeam.Entities;
//using DevTeam.Infrastructure;
//using MediatR;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using SharedKernel.Interfaces;

using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Domain.Entities;
using MediatR;
 

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Interfaces;
using SharedKernel.Repositories;

namespace DevTeam.Services.Identity.Application.Commands.Auth
{

    //LoginResponsDto => Response
    public class LoginCommand : IRequest<LoginResponseDto>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IRepository<User> _repository;//to select user  _repository.Query()
        private readonly IPasswordHasher<User> __passwordHasher;//to compare between Password from request VS PasswordHash from DB
        private readonly IJwtService _jwtService; // to
        public LoginCommandHandler(IRepository<User> repository, IPasswordHasher<User> passwordHasher, IJwtService jwtService)
        {
            _repository = repository;
            __passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            //Get user + permission
            var user = await _repository.Query().Include(x => x.UserPermissions).ThenInclude(x => x.Permission).FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken);


            //check user 
            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid username or password.");
            }

            //check password
            //user.passwordHash => user save in Db 
            //request,Password => password from client
            var passwordResault = __passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            //Get Permissions
            //var permissions = user.UserPermissions.Select(x => x.Permission.Name).Distinct().ToList();

            ////Generate token 
            //var token = _jwtService.GenerateToken(user, permissions);

            var permissions = user.UserPermissions.Select(x => x.Permission.Name).Distinct().ToList();

            var sharedUser = new SharedKernel.User
            {
                Id = user.Id,
                Username = user.Username
            };

            var token = _jwtService.GenerateToken(
                sharedUser,
                permissions);

            //return response 
            return new LoginResponseDto
            {
                Token = token,
                User = new DTOs.UserInfoDto
                {
                    Id = user.Id,
                    Permissions = permissions,
                    Username = user.Username

                }
            };

        }
    }
}

