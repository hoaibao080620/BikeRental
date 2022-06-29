using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeStationColor : BaseEntity
{
    public string Color { get; set; } = null!;
    public int BikeStationId { get; set; }
    public BikeStation BikeStation { get; set; } = null!;
    public int ManagerId { get; set; }
    public Manager Manager { get; set; } = null!;
}
