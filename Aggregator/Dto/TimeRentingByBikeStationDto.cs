namespace Aggregator.Dto;

public class TimeRentingByBikeStationDto
{
    public int BikeStationId { get; set; }
    public string BikeStationName { get; set; } = null!;
    public double Percentage { get; set; }
}
