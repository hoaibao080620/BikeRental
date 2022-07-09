namespace BikeTrackingService.Dtos;

public class BikeTrackingRetrieveDto
{
    public int BikeId { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string? Description { get; set; }
    public int? BikeStationId { get; set; }
    public string? BikeStationName { get; set; }
    public string? LastAddress { get; set; }
    public double? LastLongitude { get; set; }
    public double? LastLatitude { get; set; }
    public bool IsRenting { get; set; }
    public string? BikeStationColor { get; set; }
}
