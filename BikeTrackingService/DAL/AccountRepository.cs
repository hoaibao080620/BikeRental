using BikeTrackingService.BikeTrackingServiceDbContext;
using BikeTrackingService.Models;
using Shared.Repositories;

namespace BikeTrackingService.DAL;

public class AccountRepository : RepositoryGeneric<Account, BikeTrackingDbContext>, IAccountRepository
{
    public AccountRepository(BikeTrackingDbContext context) : base(context)
    {
    }
}
