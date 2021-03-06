namespace BikeBookingService.Dtos.BikeOperation;

public class BikeCheckinDto
{
    public int BikeId { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public DateTime CheckinTime { get; set; } = DateTime.UtcNow;
}
