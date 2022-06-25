namespace BikeService.Sonic.Dtos.BikeOperation;

public class BikeCacheParameter
{
    public int BikeId { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string? Address { get; set; }
    public string Email { get; set; } = null!;
}
