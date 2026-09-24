using DevTeam.Services.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevTeam.Services.Identity.Infrastructure.Data
{
    public class IdentityAppDbContext : DbContext
    {

        public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
    }
}
