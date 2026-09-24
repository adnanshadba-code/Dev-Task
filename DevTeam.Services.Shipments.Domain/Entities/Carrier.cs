

using SharedKernel.Classes;
namespace DevTeam.Services.Shipments.Domain.Entities;

public class Carrier : BaseEntitiy
{

    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? ServiceLevels { get; set; } 

    // Navigation Property
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

}

