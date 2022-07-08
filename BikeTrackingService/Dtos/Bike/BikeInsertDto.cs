namespace BikeService.Sonic.Dtos.Bike;

public class BikeInsertDto
{
    public string LicensePlate { get; set; } = null!;
    public string? Description { get; set; }
    public int? BikeStationId { get; set; }
}