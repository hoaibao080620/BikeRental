using BikeTrackingService.BikeTrackingServiceDbContext;
using BikeTrackingService.Models;
using Shared.Repositories;

namespace BikeTrackingService.DAL;

public class BikeRepository : RepositoryGeneric<Bike, BikeTrackingDbContext>, IBikeRepository
{
    public BikeRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
