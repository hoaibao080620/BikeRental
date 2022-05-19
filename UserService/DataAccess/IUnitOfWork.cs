namespace UserService.DataAccess;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    Task<int> SaveChangesAsync();
}