
namespace DevTeam.Services.Identity.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public UserInfoDto User { get; set; }
    }
}
