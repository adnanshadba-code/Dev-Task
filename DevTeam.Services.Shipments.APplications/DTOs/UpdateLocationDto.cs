namespace DevTeam.Services.Shipments.Applications.DTOs
{
    public class UpdateLocationDto
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string? UpdateBy { get; set; }
    }
}
