using AccountService.AccountDbContext;
using AccountService.Models;
using Shared.Repositories;

namespace AccountService.DataAccess.Implementation;

public class UserRepository : RepositoryGeneric<User, AccountServiceDbContext>, IUserRepository
{
    public UserRepository(AccountServiceDbContext context) : base(context)
    {
    }
}