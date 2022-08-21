using BikeBookingService.DAL;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;

namespace BikeBookingService.MessageQueue.Handlers;

public class UserDeletedEventHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public UserDeletedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(string message)
    {
        var userDeleted = JsonConvert.DeserializeObject<UserDeleted>(message);

        if(userDeleted is null) return;

        var account =
            (await _unitOfWork.AccountRepository.Find(x => x.ExternalId == userDeleted.UserId)).FirstOrDefault();
        
        if(account is null) return;

        account.IsActive = false;
        await _unitOfWork.SaveChangesAsync();
    }
}
