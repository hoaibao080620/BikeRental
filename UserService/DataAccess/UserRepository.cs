using Shared.Repositories;
using UserService.ApplicationDbContext;
using UserService.Models;

namespace UserService.DataAccess;

public class UserRepository : RepositoryGeneric<User, UserServiceDbContext>, IUserRepository
{
    public UserRepository(UserServiceDbContext context) : base(context)
    {
    }
}