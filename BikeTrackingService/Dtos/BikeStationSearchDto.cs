namespace BikeService.Sonic.Dtos;

public class BikeStationSearchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string NameNormalize { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string AddressNormalize { get; set; } = null!;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string? Description { get; set; }
    public string? DescriptionNormalize { get; set; }
    public int ParkingSpace { get; set; }
    public int UsedParkingSpace { get; set; }
}