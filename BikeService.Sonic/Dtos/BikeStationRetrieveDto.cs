using BikeService.Sonic.Dtos.Bike;

namespace BikeService.Sonic.Dtos;

public class BikeStationRetrieveDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string? Description { get; set; }
    public int ParkingSpace { get; set; }
    public int UsedParkingSpace { get; set; }
    public List<BikeRetrieveDto>? Bikes { get; set; }
    public string? Color { get; set; }
    public double Distance { get; set; }
    public List<int> ManagerIds { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string BikeStationCode { get; set; } = null!;
}
