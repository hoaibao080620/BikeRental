using BikeRental.MessageQueue.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace BikeRental.MessageQueue.SubscriptionManager;

public class MessageQueueSubscriptionManager : IMessageQueueSubscriptionManager
{
    private readonly Dictionary<string, Type> _messageQueueHandlers = new();

    public void RegisterEventHandlerSubscription<T>(string messageType)
    {
        if (_messageQueueHandlers.ContainsKey(messageType)) throw new Exception("This event has already register");
        _messageQueueHandlers.Add(messageType, typeof(T));
    }

    public Type GetHandler(string messageType)
    {
        return !_messageQueueHandlers.ContainsKey(messageType) ? 
            typeof(EmptyMessageQueueHandler) : 
            _messageQueueHandlers[messageType];
    }
}
