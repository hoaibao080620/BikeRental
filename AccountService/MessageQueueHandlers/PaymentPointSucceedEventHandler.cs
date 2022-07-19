using AccountService.DataAccess;
using AccountService.Models;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace AccountService.MessageQueueHandlers;

public class PaymentPointSucceedEventHandler : IMessageQueueHandler
{
    private readonly IMongoService _mongoService;

    public PaymentPointSucceedEventHandler(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<PaymentSucceeded>(message);
        if(payload is null) return;
        
        var account = (await _mongoService
                .FindAccounts(x => x.Email == payload.Email))
            .FirstOrDefault();

        if (account is null) return;
        
        var updateBuilder = Builders<Account>.Update
            .Set(x => x.Point, account.Point + payload.Amount / 1000.0);

        await _mongoService.UpdateAccount(account.Id, updateBuilder);
        await _mongoService.AddAccountTransaction(new AccountTransaction
        {
            AccountEmail = account.Email,
            Amount = payload.Amount,
            CreatedOn = DateTime.UtcNow,
            TransactionTime = DateTime.UtcNow,
            Status = "Success"
        });

        await _mongoService.AddAccountPointHistory(new AccountPointHistory
        {
            AccountEmail = account.Email,
            Point = payload.Amount/ 1000,
            CreatedOn = DateTime.UtcNow,
            AccountPhoneNumber = account.PhoneNumber
        });
    }
}
