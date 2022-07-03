using AccountService.Models;

namespace AccountService.DataAccess;

public interface IMongoService
{
    Task AddAccount(Account account);
}
