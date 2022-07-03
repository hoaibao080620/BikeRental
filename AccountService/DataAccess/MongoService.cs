using AccountService.Models;
using MongoDB.Driver;

namespace AccountService.DataAccess;

public class MongoService : IMongoService
{
    private readonly IMongoCollection<Account> _accountCollection;
    private readonly IMongoCollection<AccountTransaction> _transactionCollection;

    public MongoService(IMongoDatabase mongoDatabase)
    {
        _accountCollection = mongoDatabase.GetCollection<Account>("Account");
        _transactionCollection = mongoDatabase.GetCollection<AccountTransaction>("AccountTransaction");
    }

    public async Task AddAccount(Account account)
    {
        await _accountCollection.InsertOneAsync(account);
    }
}
