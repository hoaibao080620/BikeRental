using Shared.Repositories;
using UserService.Models;

namespace UserService.DataAccess;

public interface IUserRepository : IRepositoryGeneric<User>
{
    
}