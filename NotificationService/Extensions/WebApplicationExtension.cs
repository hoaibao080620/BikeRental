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
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.NotifyBikeLocationChange);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeCheckinCommandHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.NotifyBikeCheckin);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeCheckoutCommandHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.NotifyBikeCheckout);
    }
}
