namespace BikeService.Sonic.Dtos.BikeOperation;

public class BikeCheckoutDto
{
    public int BikeId { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int BikeStationId { get; set; }
    public DateTime CheckoutOn { get; set; }
    public string? Address { get; set; }
}
