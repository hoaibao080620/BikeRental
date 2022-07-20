using AccountService.DataAccess;
using AccountService.Models;
using AccountService.Publisher;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeRental.MessageQueue.MessageType;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace AccountService.MessageQueueHandlers;

public class PaymentPointSucceedEventHandler : IMessageQueueHandler
{
    private readonly IMongoService _mongoService;
    private readonly IMessageQueuePublisher _messageQueuePublisher;

    public PaymentPointSucceedEventHandler(IMongoService mongoService, IMessageQueuePublisher messageQueuePublisher)
    {
        _mongoService = mongoService;
        _messageQueuePublisher = messageQueuePublisher;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<PaymentSucceeded>(message);
        if(payload is null) return;
        
        var account = (await _mongoService
                .FindAccounts(x => x.Email == payload.Email))
            .FirstOrDefault();

        if (account is null) return;

        var newPoint = account.Point + payload.Amount / 1000.0;
        var updateBuilder = Builders<Account>.Update
            .Set(x => x.Point, newPoint);

        await _mongoService.UpdateAccount(account.Id, updateBuilder);
        await _mongoService.AddAccountTransaction(new AccountTransaction
        {
            AccountEmail = account.Email,
            Amount = payload.Amount,
            CreatedOn = DateTime.UtcNow,
            TransactionTime = DateTime.UtcNow,
            Status = "Success",
            AccountPhoneNumber = account.PhoneNumber
        });

        await _mongoService.AddAccountPointHistory(new AccountPointHistory
        {
            AccountEmail = account.Email,
            Point = payload.Amount/ 1000,
            CreatedOn = DateTime.UtcNow,
            AccountPhoneNumber = account.PhoneNumber
        });

        if (newPoint >= 0)
        {
            await _messageQueuePublisher.PublishAccountDebtHasBeenPaidEvent(new AccountDebtHasBeenPaid
            {
                AccountEmail = account.Email,
                MessageType = MessageType.AccountDebtHasBeenPaid
            });
        }
    }
}
