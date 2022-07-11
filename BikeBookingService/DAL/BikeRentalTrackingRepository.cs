using BikeBookingService.BikeTrackingServiceDbContext;
using BikeBookingService.Models;
using BikeService.Sonic.DAL;
using Shared.Repositories;

namespace BikeBookingService.DAL;

public class BikeRentalTrackingRepository : RepositoryGeneric<BikeRentalBooking, BikeTrackingDbContext>, IBikeRentalTrackingRepository
{
    public BikeRentalTrackingRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
