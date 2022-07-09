using BikeService.Sonic.DAL;
using BikeTrackingService.BikeTrackingServiceDbContext;
using BikeTrackingService.Models;
using Shared.Repositories;

namespace BikeTrackingService.DAL;

public class BikeLocationTrackingHistoryRepository : 
    RepositoryGeneric<BikeLocationTrackingHistory, BikeTrackingDbContext>, 
    IBikeLocationTrackingHistoryRepository
{
    public BikeLocationTrackingHistoryRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
