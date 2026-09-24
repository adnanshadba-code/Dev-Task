namespace DevTeam.Services.Shipments.Applications.DTOs
{
    public class ShipmentDto
    {
        public int Id { get; set; }

        public string TrackingNumber { get; set; } = string.Empty;

        public string Origin { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public decimal Weight { get; set; }

        public string Status { get; set; } = string.Empty;

        public int CarrierId { get; set; }

        public string CarrierName { get; set; } = string.Empty;

        public List<ShipmentEventDto> Events { get; set; } = new();
    }
}
