using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeOperation;
using BikeService.Sonic.Dtos.History;

namespace BikeService.Sonic.BusinessLogics;

public interface IBikeBusinessLogic
{
    Task<BikeRetrieveDto?> GetBike(int id);
    Task<List<BikeRetrieveDto>> GetBikes(string managerEmail);
    Task AddBike(BikeInsertDto bikeInsertDto);
    Task UpdateBike(BikeUpdateDto bikeInsertDto);
    Task DeleteBike(int id);
    Task BikeChecking(BikeCheckinDto bikeCheckinDto, string accountEmail);
    Task BikeCheckout(BikeCheckoutDto bikeCheckingDto, string accountEmail);
    Task UpdateBikeLocation(BikeLocationDto bikeLocationDto);
    Task<BikeRentingStatus> GetBikeRentingStatus(string accountEmail);
    Task DeleteBikes(List<int> bikeIds);
    Task<List<BikeRentingHistory>> GetBikeRentingHistories(string email);
}
