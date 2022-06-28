using System.Linq.Expressions;
using UserService.Models;

namespace UserService.DataAccess;

public interface IMongoService
{
    Task<List<Role>> GetRoles();
    Task AddRole(Role role);
    Task AddUser(User user);
    Task<User?> FindUser(Expression<Func<User, bool>> expression);
}
