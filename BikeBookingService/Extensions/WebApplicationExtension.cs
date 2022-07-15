using BikeBookingService.MessageQueue.Handlers;
using BikeRental.MessageQueue.MessageType;
using BikeRental.MessageQueue.SubscriptionManager;

namespace BikeBookingService.Extensions;

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
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeStationColorUpdatedHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.BikeStationColorUpdated);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserCreatedEventHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.UserAdded);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserDeletedEventHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.UserDeleted);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserRoleUpdatedHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.UserRoleUpdated);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<AccountPointLimitExceededHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.AccountPointLimitExceeded);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<AccountPointSubtractedHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.AccountPointSubtracted);
    }
}
