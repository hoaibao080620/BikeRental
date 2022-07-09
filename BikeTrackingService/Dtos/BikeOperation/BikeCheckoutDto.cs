namespace BikeTrackingService.Dtos.BikeOperation;

public class BikeCheckoutDto
{
    public int BikeId { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int? BikeStationId { get; set; } = null;
    public DateTime CheckoutOn { get; set; } = DateTime.UtcNow;
}
