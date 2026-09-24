using SharedKernel.Classes;

namespace DevTeam.Services.Shipments.Domain.Entities
{
    public class Location : BaseEntitiy
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string? Country { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }


        // Navigation Property
        public ICollection<ShipmentEvent> ShipmentEvents { get; set; }
            = new List<ShipmentEvent>();
    }
}
