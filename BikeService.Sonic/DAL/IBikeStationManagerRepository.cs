using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public interface IBikeStationManagerRepository : IRepositoryGeneric<BikeStationManager>
{
    Task<List<string>> GetManagerEmailsByBikeId(int bikeId);
}