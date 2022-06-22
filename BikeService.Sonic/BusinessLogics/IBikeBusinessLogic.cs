using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeOperation;

namespace BikeService.Sonic.BusinessLogics;

public interface IBikeBusinessLogic
{
    Task<BikeRetrieveDto?> GetBike(int id);
    Task<List<BikeRetrieveDto>> GetBikes();
    Task AddBike(BikeInsertDto bikeInsertDto);
    Task UpdateBike(BikeUpdateDto bikeInsertDto);
    Task DeleteBike(int id);
    Task BikeChecking(BikeCheckinDto bikeCheckinDto, string userEmail);
    Task BikeCheckout(BikeCheckoutDto bikeCheckingDto, string userEmail);
    Task UpdateBikeLocation(BikeLocationDto bikeLocationDto);
}
