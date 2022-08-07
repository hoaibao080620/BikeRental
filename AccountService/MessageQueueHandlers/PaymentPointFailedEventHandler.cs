using AccountService.DataAccess;
using AccountService.Models;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;

namespace AccountService.MessageQueueHandlers;

public class PaymentPointFailedEventHandler : IMessageQueueHandler
{
    private readonly IMongoService _mongoService;

    public PaymentPointFailedEventHandler(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<PaymentFailed>(message);
        if(payload is null) return;
        
        var account = (await _mongoService
                .FindAccounts(x => x.Email == payload.Email))
            .FirstOrDefault();

        if (account is null) return;
        
        await _mongoService.AddAccountTransaction(new AccountTransaction
        {
            AccountEmail = account.Email,
            Amount = payload.Amount,
            CreatedOn = DateTime.UtcNow,
            TransactionTime = DateTime.UtcNow,
            Status = "Failed",
            AccountPhoneNumber = account.PhoneNumber
        });
    }
}
