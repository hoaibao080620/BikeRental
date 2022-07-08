namespace BikeService.Sonic.Dtos;

public class BikeManipulateDto
{
    public string LicensePlate { get; set; } = null!;
    public string? Description { get; set; }
    public string BikeStationId { get; set; } = null!;
}