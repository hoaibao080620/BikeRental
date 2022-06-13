using UserService.ApplicationDbContext;

namespace UserService.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly UserServiceDbContext _serviceDbContext;
    public IUserRepository UserRepository { get; }
    public IRoleRepository RoleRepository { get; }

    public UnitOfWork(UserServiceDbContext serviceDbContext)
    {
        _serviceDbContext = serviceDbContext;
        UserRepository ??= new UserRepository(serviceDbContext);
        RoleRepository ??= new RoleRepository(serviceDbContext);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _serviceDbContext.SaveChangesAsync();
    }
}