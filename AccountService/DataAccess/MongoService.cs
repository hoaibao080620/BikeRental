using System.Linq.Expressions;
using AccountService.Models;
using MongoDB.Driver;

namespace AccountService.DataAccess;

public class MongoService : IMongoService
{
    private readonly IMongoCollection<Account> _accountCollection;
    private readonly IMongoCollection<AccountTransaction> _transactionCollection;
    private readonly IMongoCollection<AccountPointHistory> _accountPointHistory;

    public MongoService(IMongoDatabase mongoDatabase)
    {
        _accountCollection = mongoDatabase.GetCollection<Account>("Account");
        _transactionCollection = mongoDatabase.GetCollection<AccountTransaction>("AccountTransaction");
        _accountPointHistory = mongoDatabase.GetCollection<AccountPointHistory>("AccountPointHistory");
    }

    public async Task AddAccount(Account account)
    {
        await _accountCollection.InsertOneAsync(account);
    }

    public async Task<List<Account>> FindAccounts(Expression<Func<Account, bool>> expression)
    {
        return await _accountCollection.Find(expression).ToListAsync();
    }

    public async Task UpdateAccount(string accountId, UpdateDefinition<Account> builder)
    {
        await _accountCollection.UpdateOneAsync(x => x.Id == accountId, builder);
    }

    public async Task DeleteAccount(string accountId)
    {
        await _accountCollection.DeleteOneAsync(x => x.Id == accountId);
    }

    public async Task AddAccountTransaction(AccountTransaction accountTransaction)
    {
        await _transactionCollection.InsertOneAsync(accountTransaction);
    }

    public async Task<List<AccountTransaction>> FindAccountTransactions(Expression<Func<AccountTransaction, bool>> expression)
    {
        return await _transactionCollection.Find(expression).ToListAsync();
    }

    public async Task AddAccountPointHistory(AccountPointHistory accountPointHistory)
    {
        await _accountPointHistory.InsertOneAsync(accountPointHistory);
    }

    public async Task<List<AccountPointHistory>> FindAccountPointHistories(Expression<Func<AccountPointHistory, bool>> expression)
    {
        return await _accountPointHistory.Find(expression).ToListAsync();
    }
}
