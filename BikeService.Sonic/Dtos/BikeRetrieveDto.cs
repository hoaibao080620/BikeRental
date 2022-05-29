namespace BikeService.Sonic.Dtos;

public class BikeRetrieveDto
{
    public string Id { get; set; } = null!;
    public string LicensePlate { get; set; } = null!;
    public string? Description { get; set; }
    public string BikeStationId { get; set; } = null!;
}