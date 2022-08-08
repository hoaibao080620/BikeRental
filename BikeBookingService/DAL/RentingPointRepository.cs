using BikeBookingService.BikeTrackingServiceDbContext;
using BikeBookingService.Models;
using Shared.Repositories;

namespace BikeBookingService.DAL;

public class RentingPointRepository : RepositoryGeneric<RentingPoint, BikeTrackingDbContext>, IRentingPointRepository
{
    public RentingPointRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
