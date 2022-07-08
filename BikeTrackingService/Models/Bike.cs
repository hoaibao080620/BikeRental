using BikeService.Sonic.Models;
using Shared.Models;

namespace BikeTrackingService.Models;

public class Bike : BaseEntity
{
    public string LicensePlate { get; set; } = null!;
    public string? Description { get; set; }
    public int? BikeStationId { get; set; }
    public string? BikeStationName { get; set; }
    public string Status { get; set; } = null!;
    public List<BikeLocationTracking> BikeLocationTrackings { get; set; } = null!;
    public int ExternalId { get; set; }
}
