using AccountService.DataAccess;
using AccountService.Models;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;
using Shared.Consts;

namespace AccountService.MessageQueueHandlers;

public class UserCreatedEventHandler : IMessageQueueHandler
{
    private readonly IMongoService _mongoService;

    public UserCreatedEventHandler(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }
    
    public async Task Handle(string message)
    {
        var userCreatedMessage = JsonConvert.DeserializeObject<UserCreated>(message);
        if(userCreatedMessage?.Role != UserRole.User) return;
        
        await _mongoService.AddAccount(new Account
        {
            CreatedOn = DateTime.UtcNow,
            IsActive = true,
            FirstName = userCreatedMessage.FirstName,
            LastName = userCreatedMessage.LastName,
            Email = userCreatedMessage.Email,
            PhoneNumber = userCreatedMessage.PhoneNumber!,
            Point = 0,
            ExternalUserId = userCreatedMessage.Id,
            DateOfBirth = userCreatedMessage.DateOfBirth
        });
    }
}
