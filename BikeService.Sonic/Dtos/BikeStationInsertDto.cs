namespace BikeService.Sonic.Dtos;

public class BikeStationInsertDto
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string? Description { get; set; }
    public int ParkingSpace { get; set; }
    public int UsedParkingSpace { get; set; }
    public List<string>? Bikes { get; set; }
}