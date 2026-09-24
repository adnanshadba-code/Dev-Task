using DevTeam.Services.Shipments.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevTeam.Services.Shipments.Infrastructure.Data
{
    public class ConnectAppDbContext : DbContext
    {
        public ConnectAppDbContext(DbContextOptions<ConnectAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Carrier> Carriers { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<Shipment> Shipments { get; set; }

        public DbSet<ShipmentEvent> ShipmentEvents { get; set; }

       
    }
}