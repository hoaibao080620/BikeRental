namespace BikeBookingService.Dtos.BikeOperation;

public class BikeCheckinDto
{
    public string BikeCode { get; set; } = null!;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public DateTime CheckinTime { get; set; } = DateTime.UtcNow;
}
