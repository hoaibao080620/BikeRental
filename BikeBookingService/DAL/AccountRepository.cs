using BikeBookingService.BikeTrackingServiceDbContext;
using BikeBookingService.Models;
using Shared.Repositories;

namespace BikeBookingService.DAL;

public class AccountRepository : RepositoryGeneric<Account, BikeTrackingDbContext>, IAccountRepository
{
    public AccountRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
