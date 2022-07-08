using BikeTrackingService.Dtos.History;

namespace BikeTrackingService.BLL;

public interface IBikeTrackingBusinessLogic
{
    Task<List<BikeRentingHistory>> GetBikeRentingHistories(string email);
}
