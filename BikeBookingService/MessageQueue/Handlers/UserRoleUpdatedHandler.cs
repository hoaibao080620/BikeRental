using BikeBookingService.DAL;
using BikeBookingService.Models;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;
using Shared.Consts;

namespace BikeBookingService.MessageQueue.Handlers;

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

        var account = (await _unitOfWork.AccountRepository
                .Find(x => x.ExternalId == payload.UserId))
                .FirstOrDefault();

        if (payload.NewRole == UserRole.User && account is null)
        {
            await _unitOfWork.AccountRepository.Add(new Account
            {
                ExternalId = payload.UserId,
                Email = payload.Email,
                IsActive = true,
                PhoneNumber = payload.PhoneNumber!
            });

            await _unitOfWork.SaveChangesAsync();
            return;
        }

        if (account is not null && payload.NewRole != UserRole.User)
        {
            await _unitOfWork.AccountRepository.Delete(account);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
