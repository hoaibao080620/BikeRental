using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using UserService.Models;

namespace UserService.DataAccess;

public class MongoService : IMongoService
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<Role> _roleCollection;

    public MongoService(IMongoDatabase mongoDatabase)
    {
        _userCollection = mongoDatabase.GetCollection<User>("User");
        _roleCollection = mongoDatabase.GetCollection<Role>("Role");
    }

    public async Task<List<Role>> GetRoles()
    {
        var roles = await _roleCollection.FindAsync(new BsonDocument());
        return await roles.ToListAsync();
    }

    public async Task AddRole(Role role)
    {
        await _roleCollection.InsertOneAsync(role);
    }

    public async Task AddUser(User user)
    {
        await _userCollection.InsertOneAsync(user);
    }

    public async Task<User?> FindUser(Expression<Func<User, bool>> expression)
    {
        return await _userCollection.Find(expression).FirstOrDefaultAsync();
    }
}
