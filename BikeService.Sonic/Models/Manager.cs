using Shared.Models;

namespace BikeService.Sonic.Models;

public class Manager : BaseEntity
{
    public string? Email { get; set; } = null!;
    public List<BikeStationManager> BikeStationManagers { get; set; } = null!;
    public int ExternalId { get; set; }
    public bool IsSuperManager { get; set; }
}
