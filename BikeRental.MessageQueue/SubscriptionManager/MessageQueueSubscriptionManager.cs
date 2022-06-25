using BikeRental.MessageQueue.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace BikeRental.MessageQueue.SubscriptionManager;

public class MessageQueueSubscriptionManager : IMessageQueueSubscriptionManager
{
    private readonly Dictionary<string, IMessageQueueHandler> _messageQueueHandlers = new();

    public void RegisterEventHandlerSubscription<T>(IServiceProvider serviceProvider, string messageType)
    {
        if (_messageQueueHandlers.ContainsKey(messageType)) throw new Exception("This event has already register");

        if(ActivatorUtilities.CreateInstance(serviceProvider, typeof(T)) is not IMessageQueueHandler service) 
            throw new Exception("Cannot create handler instance");
        
        _messageQueueHandlers.Add(messageType, service);
    }

    public IMessageQueueHandler GetHandler(string messageType)
    {
        if (!_messageQueueHandlers.ContainsKey(messageType)) throw new Exception("This event has not register");

        return _messageQueueHandlers[messageType];
    }
}
