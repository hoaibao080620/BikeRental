using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class BikeRentalTrackingRepository : RepositoryGeneric<BikeRentalTracking, BikeServiceDbContext>, IBikeRentalTrackingRepository
{
    public BikeRentalTrackingRepository(BikeServiceDbContext context) : base(context)
    {
    }
}
