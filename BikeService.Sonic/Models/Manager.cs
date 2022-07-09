using Shared.Models;

namespace BikeService.Sonic.Models;

public class Manager : BaseEntity
{
    public string Email { get; set; } = null!;
    public List<BikeStationManager> BikeStationManagers { get; set; } = null!;
    public string ExternalId { get; set; } = null!;
    public bool IsSuperManager { get; set; }
    public List<BikeStationColor> BikeStationColors { get; set; } = null!;
}
