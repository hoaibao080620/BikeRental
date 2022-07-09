using Shared.Models;

namespace BikeTrackingService.Models;

public class RentingPrice : BaseEntity
{
    public double PointPerHour { get; set; }
    public double PointPerDay { get; set; }
    public double PointPerWeek { get; set; }
}
