namespace DevTeam.Services.Shipments.Applications.DTOs
{
    public class CarrierDto
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string ServiceLevels { get; set; } = string.Empty;
    }
}
