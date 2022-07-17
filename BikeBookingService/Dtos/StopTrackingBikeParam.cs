using BikeBookingService.Dtos.BikeOperation;

namespace BikeBookingService.Dtos;

public class StopTrackingBikeParam
{
    public BikeCheckoutDto BikeCheckoutDto { get; set; } = null!;
    public int BikeId { get; set; }
    public string AccountEmail { get; set; } = null!;
    public string Address { get; set; } = null!;
    public double RentingPoint { get; set; }
}
