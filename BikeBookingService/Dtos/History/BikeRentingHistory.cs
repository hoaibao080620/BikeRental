namespace BikeBookingService.Dtos.History;

public class BikeRentingHistory
{
    public int Id { get; set; }
    public int BikeId { get; set; }
    public string BikePlate { get; set; } = null!;
    public DateTime CheckedInOn { get; set; }
    public DateTime? CheckedOutOn { get; set; }
    public double? TotalTime { get; set; }
    public string AccountEmail { get; set; } = null!;
    public string AccountPhone { get; set; } = null!;
}
