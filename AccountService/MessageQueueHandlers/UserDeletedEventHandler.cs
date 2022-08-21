using AccountService.DataAccess;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;

namespace AccountService.MessageQueueHandlers;

public class UserDeletedEventHandler : IMessageQueueHandler
{
    private readonly IMongoService _mongoService;

    public UserDeletedEventHandler(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }
    
    public async Task Handle(string message)
    {
        var userDeleted = JsonConvert.DeserializeObject<UserDeleted>(message);

        if(userDeleted is null) return;
        
        var account = (await _mongoService
            .FindAccounts(x => x.ExternalUserId == userDeleted.UserId)).FirstOrDefault();
        
        if(account is null) return;
        
        await _mongoService.DeleteAccount(account);
    }
}
