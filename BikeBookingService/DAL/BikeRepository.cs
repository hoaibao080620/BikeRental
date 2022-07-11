using BikeBookingService.BikeTrackingServiceDbContext;
using BikeBookingService.Models;
using Shared.Repositories;

namespace BikeBookingService.DAL;

public class BikeRepository : RepositoryGeneric<Bike, BikeTrackingDbContext>, IBikeRepository
{
    public BikeRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
