using AccountService.DataAccess.Interfaces;
using AccountService.Models;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;

namespace AccountService.MessageQueueHandlers;

public class UserCreatedEventHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public UserCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(string message)
    {
        var userCreatedMessage = JsonConvert.DeserializeObject<UserCreated>(message);
        if(userCreatedMessage is null) return;

        var user = await AddUser(userCreatedMessage);
        await _unitOfWork.AccountRepository.Add(new Account
        {
            AccountCode = Guid.NewGuid(),
            CreatedOn = DateTime.UtcNow,
            IsActive = true,
            User = user
        });
        
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<User> AddUser(UserCreated userCreatedMessage)
    {
        var user = new User
        {
            ExternalId = userCreatedMessage.Id,
            FirstName = userCreatedMessage.FirstName,
            LastName = userCreatedMessage.LastName,
            PhoneNumber = userCreatedMessage.PhoneNumber,
            Email = userCreatedMessage.Email
        };
        
        await _unitOfWork.UserRepository.Add(user);
        return user;
    }
}
