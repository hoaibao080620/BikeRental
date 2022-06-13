using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public interface IBikeStationRepository : IRepositoryGeneric<BikeStation>
{
    Task<BikeStation?> GetBikeStationByName(string name);
}
