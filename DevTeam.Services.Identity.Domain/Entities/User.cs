using SharedKernel.Classes;

namespace DevTeam.Services.Identity.Domain.Entities
{
    public class User : BaseEntitiy
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<UserPermission> UserPermissions { get; set; }
    }
}
