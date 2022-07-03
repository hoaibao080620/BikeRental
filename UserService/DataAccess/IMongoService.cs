using System.Linq.Expressions;
using MongoDB.Driver;
using UserService.Models;

namespace UserService.DataAccess;

public interface IMongoService
{
    Task<List<Role>> GetRoles();
    Task AddRole(Role role);
    Task AddUser(User user);
    Task<List<User>> FindUser(Expression<Func<User, bool>> expression);
    Task DeleteUser(string id);
    Task UpdateUser(string userId, UpdateDefinition<User> builder);
}
