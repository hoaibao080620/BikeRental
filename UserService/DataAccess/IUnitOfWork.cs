namespace UserService.DataAccess;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    Task<int> SaveChangesAsync();
}