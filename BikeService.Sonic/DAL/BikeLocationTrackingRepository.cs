using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class BikeLocationTrackingRepository : RepositoryGeneric<BikeLocationTracking, BikeServiceDbContext>, IBikeLocationTrackingRepository
{
    public BikeLocationTrackingRepository(BikeServiceDbContext context) : base(context)
    {
    }
}
