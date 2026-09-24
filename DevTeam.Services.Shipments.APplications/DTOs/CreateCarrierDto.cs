namespace DevTeam.Services.Shipments.Applications.DTOs
{
    public class CreateCarrierDto
    {
        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string ServiceLevels { get; set; } = string.Empty;
    }
}
