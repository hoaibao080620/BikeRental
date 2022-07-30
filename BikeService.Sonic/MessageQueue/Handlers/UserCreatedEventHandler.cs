using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Models;
using Newtonsoft.Json;
using Shared.Consts;

namespace BikeService.Sonic.MessageQueue.Handlers;

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
        if (userCreatedMessage?.Role is UserRole.Manager or UserRole.Director)
        {
            await _unitOfWork.ManagerRepository.Add(new Manager
            {
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                Email = userCreatedMessage.Email,
                ExternalId = userCreatedMessage.Id,
                IsSuperManager = userCreatedMessage.Role == UserRole.Director
            });
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
