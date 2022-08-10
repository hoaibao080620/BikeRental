using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeStation : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string? Description { get; set; }
    public int ParkingSpace { get; set; }
    public string? Code { get; set; }
    public List<Bike> Bikes { get; set; } = null!;
    public List<BikeStationManager> BikeStationManagers { get; set; } = null!;
    public List<BikeStationColor> BikeStationColors { get; set; } = null!;
}
