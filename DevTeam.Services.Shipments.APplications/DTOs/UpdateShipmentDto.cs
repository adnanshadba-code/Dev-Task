namespace DevTeam.Services.Shipments.Applications.DTOs;

public class UpdateShipmentDto
{
    public string TrackingNumber { get; set; } = string.Empty;

    public string Origin { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public decimal Weight { get; set; }

    public string Status { get; set; } = string.Empty;

    public int CarrierId { get; set; }

    public string? UpdateBy { get; set; }
}