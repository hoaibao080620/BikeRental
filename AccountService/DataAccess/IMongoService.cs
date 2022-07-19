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
    Task AddAccountTransaction(AccountTransaction accountTransaction);
    Task<List<AccountTransaction>> FindAccountTransactions(Expression<Func<AccountTransaction, bool>> expression, int limit = 1000);
    Task AddAccountPointHistory(AccountPointHistory accountPointHistory);
    Task<List<AccountPointHistory>> FindAccountPointHistories(Expression<Func<AccountPointHistory, bool>> expression);
}
