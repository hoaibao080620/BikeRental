using BikeService.Sonic.DAL;
using BikeService.Sonic.Models;
using BikeTrackingService.BikeTrackingServiceDbContext;
using Shared.Repositories;

namespace BikeTrackingService.DAL;

public class BikeLocationTrackingRepository : RepositoryGeneric<BikeLocationTracking, BikeTrackingDbContext>, IBikeLocationTrackingRepository
{
    public BikeLocationTrackingRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
