using BikeBookingService.Dtos;
using BikeBookingService.Dtos.BikeOperation;
using BikeBookingService.Dtos.History;

namespace BikeBookingService.BLL;

public interface IBikeTrackingBusinessLogic
{
    Task<List<BikeRentingHistory>> GetBikeRentingHistories(string email);
    Task<List<BikeRentingHistory>> GetAllBikeRentingHistories();
    Task<List<BikeTrackingRetrieveDto>> GetBikesTracking(string email);
    Task BikeChecking(BikeCheckinDto bikeCheckinDto, string accountEmail);
    Task BikeCheckout(BikeCheckoutDto bikeCheckingDto, string accountEmail);
    Task UpdateBikeLocation(BikeLocationDto bikeLocationDto);
    Task<BikeRentingStatus> GetBikeRentingStatus(string accountEmail);
    Task<List<BikeBookingHistoryDto>> GetBikeBookingHistories(string accountEmail);
    Task CheckBikeRentingHasUserAlmostRunOutPoint();
    Task TestNotification(string email);
    Task<List<BikeTrackingRetrieveDto>> GetAllBikeTracking();
}
