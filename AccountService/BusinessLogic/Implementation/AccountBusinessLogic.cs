using AccountService.BusinessLogic.Interfaces;
using BikeRental.MessageQueue.Handlers;

namespace AccountService.BusinessLogic.Implementation;

public class AccountBusinessLogic : IAccountBusinessLogic
{
    public AccountBusinessLogic(IMessageQueueHandler messageQueueHandler)
    {

    }

    public Task CreateAccount(string message)
    {
        throw new NotImplementedException();
    }
}
