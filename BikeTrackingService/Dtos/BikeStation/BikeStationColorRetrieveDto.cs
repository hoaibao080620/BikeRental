namespace BikeService.Sonic.Dtos.BikeStation;

public class BikeStationColorRetrieveDto
{
    public int BikeStationId { get; set; }
    public string BikeStationName { get; set; } = null!;
    public string? Color { get; set; }
}
