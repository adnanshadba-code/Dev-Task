namespace DevTeam.Services.Identity.Application.DTOs
{
    public class UpdateUserDto
    {
        public string Username { get; set; } = string.Empty;

        public string? Password { get; set; }

        public string? UpdateBy { get; set; }
    }
}
