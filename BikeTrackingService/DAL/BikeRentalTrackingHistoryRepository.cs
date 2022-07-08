using BikeService.Sonic.DAL;
using BikeService.Sonic.Models;
using BikeTrackingService.BikeTrackingServiceDbContext;
using Shared.Repositories;

namespace BikeTrackingService.DAL;

public class BikeRentalTrackingHistoryRepository : 
    RepositoryGeneric<BikeLocationTrackingHistory, BikeTrackingDbContext>, 
    IBikeRentalTrackingHistoryRepository
{
    public BikeRentalTrackingHistoryRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
