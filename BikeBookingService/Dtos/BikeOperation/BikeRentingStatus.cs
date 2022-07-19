namespace BikeBookingService.Dtos.BikeOperation;

public class BikeRentingStatus
{
    public string AccountEmail { get; set; } = null!;
    public int? BikeId { get; set; }
    public string? LicensePlate { get; set; } = null!;
    public bool IsRenting { get; set; }
    public int TimeUsing { get; set; }
    public double? Cost { get; set; }
}
