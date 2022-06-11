using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.Bike;

namespace BikeService.Sonic.BusinessLogics;

public interface IBikeBusinessLogic
{
    Task<BikeRetrieveDto> GetBike(int id);
    Task<List<BikeRetrieveDto>> GetBikes();
    Task AddBike(BikeInsertDto bikeInsertDto);
    Task UpdateBike(BikeUpdateDto bikeInsertDto);
    Task DeleteBike(int id);
    Task BikeChecking(BikeCheckingDto bikeCheckingDto);
    Task BikeCheckout(BikeCheckoutDto bikeCheckingDto);
    Task UpdateBikeLocation(BikeLocationDto bikeLocationDto);
}