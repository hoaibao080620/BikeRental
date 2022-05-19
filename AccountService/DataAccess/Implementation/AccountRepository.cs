using AccountService.AccountDbContext;
using AccountService.DataAccess.Interfaces;
using AccountService.Models;
using Shared.Repositories;

namespace AccountService.DataAccess.Implementation;

public class AccountRepository : RepositoryGeneric<Account, AccountServiceDbContext>, IAccountRepository
{
    public AccountRepository(AccountServiceDbContext context) : base(context)
    {
    }
}