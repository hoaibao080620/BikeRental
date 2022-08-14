namespace BikeRental.MessageQueue.SubscriptionManager;

public interface IMessageQueueSubscriptionManager
{
    void RegisterEventHandlerSubscription<T>(string messageType);
    Type GetHandler(string messageType);
}
