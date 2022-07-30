using AccountService.DataAccess;
using AccountService.Models;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace AccountService.MessageQueueHandlers;

public class UserReactivatedEventHandler : IMessageQueueHandler
{
    private readonly IMongoService _mongoService;

    public UserReactivatedEventHandler(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }
    
    public async Task Handle(string message)
    {
        var userUpdatedMessage = JsonConvert.DeserializeObject<UserDeactivated>(message);
        if(userUpdatedMessage is null) return;

        var account = (await _mongoService
            .FindAccounts(x => x.ExternalUserId == userUpdatedMessage.UserId)).FirstOrDefault();
        
        if(account is null) return;

        var builder = Builders<Account>.Update
            .Set(x => x.IsActive, true);
    }
}
