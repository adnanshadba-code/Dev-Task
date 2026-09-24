namespace DevTeam.Services.Identity.Application.DTOs
{
    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;

        public string Pswword { get; set; } = string.Empty;
        public string? CreateBy { get; set; }

        public List<int> PermissionIds { get; set; }
    }
}
