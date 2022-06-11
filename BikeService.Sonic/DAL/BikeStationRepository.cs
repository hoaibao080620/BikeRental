using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class BikeStationRepository : RepositoryGeneric<BikeStation, BikeServiceDbContext>, IBikeStationRepository
{
    public BikeStationRepository(BikeServiceDbContext context) : base(context)
    {
    }
}
