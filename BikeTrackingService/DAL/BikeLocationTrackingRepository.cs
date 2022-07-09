using BikeService.Sonic.DAL;
using BikeTrackingService.BikeTrackingServiceDbContext;
using BikeTrackingService.Models;
using Shared.Repositories;

namespace BikeTrackingService.DAL;

public class BikeLocationTrackingRepository : RepositoryGeneric<BikeLocationTracking, BikeTrackingDbContext>, IBikeLocationTrackingRepository
{
    public BikeLocationTrackingRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
