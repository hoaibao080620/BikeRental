using BikeRental.MessageQueue.Handlers;

namespace BikeRental.MessageQueue.SubscriptionManager;

public interface IMessageQueueSubscriptionManager
{
    void RegisterEventHandlerSubscription<T>(IServiceProvider serviceProvider, string messageType);
    IMessageQueueHandler GetHandler(string messageType);
}