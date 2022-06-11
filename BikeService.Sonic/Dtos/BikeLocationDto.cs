namespace BikeService.Sonic.Dtos;

public class BikeLocationDto
{
    public int BikeId { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string? Plate { get; set; }
    public string? Operation { get; set; }
}