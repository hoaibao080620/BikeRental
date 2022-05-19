namespace AccountService.DataAccess.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IAccountRepository AccountRepository { get; }
    Task<int> SaveChangesAsync();
}