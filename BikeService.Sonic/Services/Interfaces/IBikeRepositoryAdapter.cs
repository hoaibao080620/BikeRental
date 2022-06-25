using BikeService.Sonic.Dtos.Bike;

namespace BikeService.Sonic.Services.Interfaces;

public interface IBikeRepositoryAdapter
{
    Task<BikeRetrieveDto?> GetBike(int bikeId);
    Task<List<BikeRetrieveDto>> GetBikes(string managerEmail);
}
