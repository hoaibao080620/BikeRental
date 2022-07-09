using BikeService.Sonic.DAL;
using BikeTrackingService.BikeTrackingServiceDbContext;
using BikeTrackingService.Models;
using Shared.Repositories;

namespace BikeTrackingService.DAL;

public class BikeRentalTrackingRepository : RepositoryGeneric<BikeRentalTracking, BikeTrackingDbContext>, IBikeRentalTrackingRepository
{
    public BikeRentalTrackingRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
