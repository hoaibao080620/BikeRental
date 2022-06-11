using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class AccountRepository : RepositoryGeneric<Account, BikeServiceDbContext>, IAccountRepository
{
    public AccountRepository(BikeServiceDbContext context) : base(context)
    {
    }
}