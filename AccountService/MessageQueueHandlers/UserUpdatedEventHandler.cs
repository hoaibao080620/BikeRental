using AccountService.DataAccess;
using AccountService.Models;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace AccountService.MessageQueueHandlers;

public class UserUpdatedEventHandler : IMessageQueueHandler
{
    private readonly IMongoService _mongoService;

    public UserUpdatedEventHandler(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }
    
    public async Task Handle(string message)
    {
        var userUpdatedMessage = JsonConvert.DeserializeObject<UserUpdated>(message);
        if(userUpdatedMessage is null) return;

        var account = (await _mongoService
            .FindAccounts(x => x.ExternalUserId == userUpdatedMessage.Id)).FirstOrDefault();
        
        if(account is null) return;
        
        var builder = Builders<Account>.Update
            .Set(x => x.FirstName, userUpdatedMessage.FirstName)
            .Set(x => x.LastName, userUpdatedMessage.LastName)
            .Set(x => x.PhoneNumber, userUpdatedMessage.PhoneNumber)
            .Set(x => x.UpdatedOn, DateTime.UtcNow)
            .Set(x => x.DateOfBirth, userUpdatedMessage.DateOfBirth)
            .Set(x => x.Address, userUpdatedMessage.Address);

        await _mongoService.UpdateAccount(account.Id, builder);
    }
}
