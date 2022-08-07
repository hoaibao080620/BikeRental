using BikeRental.MessageQueue.MessageType;
using BikeRental.MessageQueue.SubscriptionManager;
using NotificationService.MessageQueue.MessageQueueHandlers;

namespace NotificationService.Extensions;

public static class WebApplicationExtension
{
    public static void RegisterMessageHandler(this WebApplication webApplication)
    {
        var serviceProvider = webApplication.Services;
        var messageQueueSubscriptionManager = serviceProvider.GetRequiredService<IMessageQueueSubscriptionManager>();
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<NotifyBikeLocationChangeCommandHandler>(
            MessageType.NotifyBikeLocationChange);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeCheckinCommandHandler>(
            MessageType.BikeCheckedIn);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeCheckoutCommandHandler>(
            MessageType.BikeCheckedOut);
    }
}
