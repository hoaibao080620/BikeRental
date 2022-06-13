using AccountService.DataAccess.Interfaces;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;

namespace AccountService.MessageQueueHandlers;

public class UserUpdatedEventHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public UserUpdatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(string message)
    {
        var userUpdatedMessage = JsonConvert.DeserializeObject<UserCreated>(message);

        if(userUpdatedMessage is null) return;

        var user = (await _unitOfWork.UserRepository
            .Find(x => x.ExternalId == userUpdatedMessage.Id)).FirstOrDefault();
        
        if(user is null) return;

        user.FirstName = user.FirstName;
        user.LastName = user.LastName;
        user.PhoneNumber = user.PhoneNumber;
    }
}