using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Models;
using Newtonsoft.Json;
using Shared.Consts;

namespace BikeService.Sonic.MessageQueue.Handlers;

public class UserRoleUpdatedHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public UserRoleUpdatedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<UserRoleUpdated>(message);

        if (payload is null) return;

        var manager = (await _unitOfWork.ManagerRepository
                .Find(x => x.ExternalId == payload.UserId))
                .FirstOrDefault();
        
        if (payload.NewRole is UserRole.Manager or UserRole.SuperManager)
        {
            if (manager is null)
            {
                await _unitOfWork.ManagerRepository.Add(new Manager
                {
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    Email = payload.Email,
                    ExternalId = payload.UserId,
                    IsSuperManager = payload.NewRole == UserRole.SuperManager
                });
            }
            else
            {
                manager.IsSuperManager = payload.NewRole == UserRole.SuperManager;
            }

            await _unitOfWork.SaveChangesAsync();
            return;
        }

        if (manager is not null && (payload.NewRole != UserRole.Manager || payload.NewRole != UserRole.SuperManager))
        {
            await _unitOfWork.ManagerRepository.Delete(manager);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
