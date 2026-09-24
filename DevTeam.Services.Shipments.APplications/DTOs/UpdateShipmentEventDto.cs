namespace DevTeam.Services.Shipments.Applications.DTOs;

public class UpdateShipmentEventDto
{
    public string Status { get; set; } = string.Empty;

    public int ShipmentId { get; set; }

    public int LocationId { get; set; }

    public string? UpdateBy { get; set; }
}