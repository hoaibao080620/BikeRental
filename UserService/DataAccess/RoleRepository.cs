using Shared.Repositories;
using UserService.ApplicationDbContext;
using UserService.Models;

namespace UserService.DataAccess;

public class RoleRepository : RepositoryGeneric<Role, UserServiceDbContext>, IRoleRepository
{
    public RoleRepository(UserServiceDbContext context) : base(context)
    {
    }
}