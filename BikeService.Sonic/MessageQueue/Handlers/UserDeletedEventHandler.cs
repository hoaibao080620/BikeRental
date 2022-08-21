using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeService.Sonic.DAL;
using Newtonsoft.Json;

namespace BikeService.Sonic.MessageQueue.Handlers;

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

        var manager =
            (await _unitOfWork.ManagerRepository.Find(x => x.ExternalId == userDeleted.UserId)).FirstOrDefault();
        
        if(manager is null) return;

        var bikeStationManagers = 
            await _unitOfWork.BikeStationManagerRepository.Find(x => x.Manager.ExternalId == userDeleted.UserId);
        bikeStationManagers.ToList().ForEach(x => _unitOfWork.BikeStationManagerRepository.Delete(x));
        manager.IsActive = false;
        await _unitOfWork.SaveChangesAsync();
    }
}
