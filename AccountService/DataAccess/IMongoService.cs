using System.Linq.Expressions;
using AccountService.Models;
using MongoDB.Driver;

namespace AccountService.DataAccess;

public interface IMongoService
{
    Task AddAccount(Account account);
    Task<List<Account>> FindAccounts(Expression<Func<Account, bool>> expression);
    Task UpdateAccount(string accountId, UpdateDefinition<Account> builder);
    Task DeleteAccount(string accountId);
}
