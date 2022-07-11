namespace BikeService.Sonic.Dtos.BikeOperation;

public class BikeRentingStatus
{
    public string AccountEmail { get; set; } = null!;
    public int? BikeId { get; set; }
    public string? LicensePlate { get; set; } = null!;
    public bool IsRenting { get; set; }
    public double? LastLongitude { get; set; }
    public double? LastLatitude { get; set; }
    public string? LastAddress { get; set; }
}
