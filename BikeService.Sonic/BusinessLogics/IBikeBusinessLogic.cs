using BikeService.Sonic.Dtos.Bike;

namespace BikeService.Sonic.BusinessLogics;

public interface IBikeBusinessLogic
{
    Task<BikeRetrieveDto?> GetBike(string id);
    Task<List<BikeRetrieveDto>> GetBikes(string managerEmail);
    Task AddBike(BikeInsertDto bikeInsertDto);
    Task UpdateBike(BikeUpdateDto bikeInsertDto);
    Task DeleteBike(int id);
    Task DeleteBikes(List<int> bikeIds);
    Task UnlockBike(int bikeId);
    Task<int> GetCurrentRentingBike(string phoneNumber);
    Task<List<BikeRetrieveDto>> GetAllBikes();
}
