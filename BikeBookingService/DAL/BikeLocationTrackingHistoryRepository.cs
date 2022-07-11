using BikeBookingService.BikeTrackingServiceDbContext;
using BikeBookingService.Models;
using BikeService.Sonic.DAL;
using Shared.Repositories;

namespace BikeBookingService.DAL;

public class BikeLocationTrackingHistoryRepository : 
    RepositoryGeneric<BikeLocationTrackingHistory, BikeTrackingDbContext>, 
    IBikeLocationTrackingHistoryRepository
{
    public BikeLocationTrackingHistoryRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
