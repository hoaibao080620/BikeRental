using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class BikeStationColorRepository : RepositoryGeneric<BikeStationColor, BikeServiceDbContext>, IBikeStationColorRepository
{
    public BikeStationColorRepository(BikeServiceDbContext context) : base(context)
    {
    }
}
