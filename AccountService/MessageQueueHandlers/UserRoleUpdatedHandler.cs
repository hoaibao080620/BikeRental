using AccountService.DataAccess;
using AccountService.Models;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;
using Shared.Consts;

namespace AccountService.MessageQueueHandlers;

public class UserRoleUpdatedHandler : IMessageQueueHandler
{
    private readonly IMongoService _mongoService;

    public UserRoleUpdatedHandler(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<UserRoleUpdated>(message);

        if (payload is null) return;

        var account = (await _mongoService
                .FindAccounts(x => x.ExternalUserId == payload.UserId))
                .FirstOrDefault();

        if (payload.NewRole == UserRole.User && account is null)
        {
            await _mongoService.AddAccount(new Account
            {
                ExternalUserId = payload.UserId,
                Email = payload.Email,
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                IsActive = true,
                Point = 0,
                PhoneNumber = payload.PhoneNumber
            });
            return;
        }

        if (account is not null && payload.NewRole != UserRole.User)
        {
            await _mongoService.DeleteAccount(account.Id);
        }
    }
}
