using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class BikeRentalBookingRepository : RepositoryGeneric<BikeRentalBooking, BikeServiceDbContext>, IBikeRentalBookingRepository
{
    public BikeRentalBookingRepository(BikeServiceDbContext context) : base(context)
    {
    }
}
