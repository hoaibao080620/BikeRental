using BikeService.Sonic.Dtos.BikeOperation;
using BikeTrackingService.Dtos;
using BikeTrackingService.Dtos.BikeOperation;
using BikeTrackingService.Dtos.History;

namespace BikeTrackingService.BLL;

public interface IBikeTrackingBusinessLogic
{
    Task<List<BikeRentingHistory>> GetBikeRentingHistories(string email);
    Task<List<BikeTrackingRetrieveDto>> GetBikesTracking(string email);
    Task BikeChecking(BikeCheckinDto bikeCheckinDto, string accountEmail);
    Task BikeCheckout(BikeCheckoutDto bikeCheckingDto, string accountEmail);
    Task UpdateBikeLocation(BikeLocationDto bikeLocationDto);
}
