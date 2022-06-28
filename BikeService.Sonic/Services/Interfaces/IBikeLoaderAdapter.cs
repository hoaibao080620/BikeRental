using BikeService.Sonic.Dtos.Bike;

namespace BikeService.Sonic.Services.Interfaces;

public interface IBikeLoaderAdapter
{
    Task<BikeRetrieveDto> GetBike(int bikeId);
    Task<List<int>> GetBikeIdsOfManager(string managerEmail);
}
