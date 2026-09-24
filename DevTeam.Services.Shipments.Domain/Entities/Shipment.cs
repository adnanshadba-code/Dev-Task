using SharedKernel.Classes;

namespace DevTeam.Services.Shipments.Domain.Entities;

public class Shipment : BaseEntitiy
{

    public string TrackingNumber { get; set; }

    public string? Origin { get; set; } 

    public string? Destination { get; set; }

    public decimal? Weight { get; set; }

    public string? Status { get; set; } 


    // Foreign Key
    public int CarrierId { get; set; }


    // Navigation Property
    public Carrier Carrier { get; set; } 


    // Navigation Property
    public ICollection<ShipmentEvent> ShipmentEvents { get; set; }
        = new List<ShipmentEvent>();
}