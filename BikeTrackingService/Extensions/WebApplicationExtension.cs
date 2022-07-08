using BikeRental.MessageQueue.MessageType;
using BikeRental.MessageQueue.SubscriptionManager;
using BikeTrackingService.MessageQueue.Handlers;

namespace BikeTrackingService.Extensions;

public static class WebApplicationExtension
{
    public static void RegisterMessageHandler(this WebApplication webApplication)
    {
        var serviceProvider = webApplication.Services;
        var messageQueueSubscriptionManager = serviceProvider.GetRequiredService<IMessageQueueSubscriptionManager>();
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeCreatedHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.BikeCreated);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeUpdatedHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.BikeUpdated);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeDeletedHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.BikeDeleted);
    }
}
