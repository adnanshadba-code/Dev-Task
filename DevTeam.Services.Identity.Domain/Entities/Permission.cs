using SharedKernel.Classes;

namespace DevTeam.Services.Identity.Domain.Entities
{
    public class Permission : BaseEntitiy
    {
        public string Name { get; set; }

        public string ?Description { get; set; }

        public ICollection<UserPermission> UserPermissions { get; set; }
    }
}
