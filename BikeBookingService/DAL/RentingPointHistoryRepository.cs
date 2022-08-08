using BikeBookingService.BikeTrackingServiceDbContext;
using BikeBookingService.Models;
using Shared.Repositories;

namespace BikeBookingService.DAL;

public class RentingPointHistoryRepository : RepositoryGeneric<RentingPointHistory, BikeTrackingDbContext>, IRentingPointHistoryRepository
{
    public RentingPointHistoryRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
