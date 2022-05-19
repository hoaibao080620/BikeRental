using AccountService.AccountDbContext;
using AccountService.DataAccess.Interfaces;

namespace AccountService.DataAccess.Implementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccountServiceDbContext _serviceDbContext;
    public IUserRepository UserRepository { get; }
    public IAccountRepository AccountRepository { get; }

    public UnitOfWork(AccountServiceDbContext serviceDbContext)
    {
        _serviceDbContext = serviceDbContext;
        UserRepository ??= new UserRepository(serviceDbContext);
        AccountRepository ??= new AccountRepository(serviceDbContext);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _serviceDbContext.SaveChangesAsync();
    }
}