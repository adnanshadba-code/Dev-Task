using SharedKernel.Classes;

namespace DevTeam.Services.Shipments.Domain.Entities
{
    public class ShipmentEvent : BaseEntitiy
    {
        public string? Status { get; set; } 


        // Foreign Key
        public int ShipmentId { get; set; }


        // Navigation Property
        public Shipment Shipment { get; set; } = null!;


        // Foreign Key
        public int LocationId { get; set; }


        // Navigation Property
        public Location Location { get; set; } = null!;

    }
}
