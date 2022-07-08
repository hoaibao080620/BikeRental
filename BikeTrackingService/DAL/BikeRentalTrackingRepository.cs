using BikeService.Sonic.DAL;
using BikeService.Sonic.Models;
using BikeTrackingService.BikeTrackingServiceDbContext;
using Shared.Repositories;

namespace BikeTrackingService.DAL;

public class BikeRentalTrackingRepository : RepositoryGeneric<BikeRentalTracking, BikeTrackingDbContext>, IBikeRentalTrackingRepository
{
    public BikeRentalTrackingRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
