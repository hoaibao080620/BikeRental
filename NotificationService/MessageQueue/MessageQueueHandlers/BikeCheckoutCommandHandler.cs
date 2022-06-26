using BikeRental.MessageQueue.Handlers;

namespace NotificationService.MessageQueue.MessageQueueHandlers;

public class BikeCheckoutCommandHandler : IMessageQueueHandler
{
    public Task Handle(string message)
    {
        Console.WriteLine("Handle");
        return Task.CompletedTask;
    }
}
