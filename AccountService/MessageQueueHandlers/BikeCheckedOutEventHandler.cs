using AccountService.DataAccess;
using AccountService.Models;
using AccountService.Publisher;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeRental.MessageQueue.MessageType;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace AccountService.MessageQueueHandlers;

public class BikeCheckedOutEventHandler : IMessageQueueHandler
{
    private readonly IMongoService _mongoService;
    private readonly IMessageQueuePublisher _messageQueuePublisher;

    public BikeCheckedOutEventHandler(IMongoService mongoService, IMessageQueuePublisher messageQueuePublisher)
    {
        _mongoService = mongoService;
        _messageQueuePublisher = messageQueuePublisher;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<BikeCheckedOut>(message);
        if (payload is null) return;

        var account = (await _mongoService
            .FindAccounts(x => x.Email == payload.AccountEmail))
            .First();

        var pointAfterMinus = account.Point - payload.RentingPoint;
        var updateBuilder = Builders<Account>.Update
            .Set(x => x.Point, pointAfterMinus);
        await _mongoService.UpdateAccount(account.Id, updateBuilder);
        
        if (pointAfterMinus < 0)
        {
            await _messageQueuePublisher.PublishAccountPointLimitExceededEvent(new AccountPointLimitExceeded
            {
                AccountId = account.Id,
                AccountEmail = account.Email,
                MessageType = MessageType.AccountPointLimitExceeded
            });
            return;
        }
        
        await _messageQueuePublisher.PublishAccountPointSubtractedEvent(new AccountPointSubtracted
        {
            AccountId = account.Id,
            AccountEmail = account.Email,
            MessageType = MessageType.AccountPointSubtracted
        });

        await _mongoService.AddAccountPointHistory(new AccountPointHistory
        {
            AccountEmail = account.Email,
            Point = payload.RentingPoint * -1,
            CreatedOn = DateTime.UtcNow,
            AccountPhoneNumber = account.Email.Split("@").First()
        });
    }
}
