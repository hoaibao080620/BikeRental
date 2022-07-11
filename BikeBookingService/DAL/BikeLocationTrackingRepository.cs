using BikeBookingService.BikeTrackingServiceDbContext;
using BikeBookingService.Models;
using BikeService.Sonic.DAL;
using Shared.Repositories;

namespace BikeBookingService.DAL;

public class BikeLocationTrackingRepository : RepositoryGeneric<BikeLocationTracking, BikeTrackingDbContext>, IBikeLocationTrackingRepository
{
    public BikeLocationTrackingRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
