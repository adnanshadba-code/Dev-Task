namespace DevTeam.Services.Shipments.Applications.DTOs;

public class ShipmentEventDto
{
    public int Id { get; set; }

    public string Status { get; set; } = string.Empty;

    public int ShipmentId { get; set; }
    public int TrackingNumber { get; set; }

    public int LocationId { get; set; }

    public string LocationName { get; set; } = string.Empty;
}