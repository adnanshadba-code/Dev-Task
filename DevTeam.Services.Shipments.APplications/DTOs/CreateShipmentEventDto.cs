namespace DevTeam.Services.Shipments.Applications.DTOs
{
    public class CreateShipmentEventDto
    {
        public string Status { get; set; } = string.Empty;

        public int ShipmentId { get; set; }

        public int LocationId { get; set; }
        public string CreateBy { get; set; }
    }
}