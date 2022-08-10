namespace BikeBookingService.Dtos.BikeOperation;

public class BikeCheckoutDto
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string Code { get; set; } = null!;
    public DateTime CheckoutOn { get; set; } = DateTime.UtcNow;
}
