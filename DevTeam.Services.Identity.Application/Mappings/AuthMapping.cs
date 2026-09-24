
using DevTeam.Services.Identity.Application.Commands.Auth;
using DevTeam.Services.Identity.Application.DTOs;

namespace DevTeam.Services.Identity.Application.Mappings
{
    public static class AuthMapping
    {

        //LoginDto
        //↓ ToCommand()
        //LoginCommand
        public static LoginCommand ToCommand(this LoginDto loginDto)
        {
            return new LoginCommand
            {
                Password = loginDto.Password,
                Username = loginDto.Username,
            };
        }

    }
}
