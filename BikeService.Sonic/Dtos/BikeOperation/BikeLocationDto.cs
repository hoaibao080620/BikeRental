namespace BikeService.Sonic.Dtos.BikeOperation;

public class BikeLocationDto
{
    public int BikeId { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string? LicensePlate { get; set; }
    public string? Operation { get; set; }
}
