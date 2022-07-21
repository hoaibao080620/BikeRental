namespace BikeBookingService.Dtos;

public class BikeBookingHistoryDto
{
    public DateTime CheckinOn { get; set; }
    public DateTime CheckoutOn { get; set; }
    public double TotalPoint { get; set; }
    public string CheckinBikeStation { get; set; } = null!;
    public string CheckoutBikeStation { get; set; } = null!;
    public string AccountPhoneNumber { get; set; } = null!;
    public string BikeLicensePlate { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public double TotalDistance { get; set; }
}
