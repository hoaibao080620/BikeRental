using BikeBookingService.BLL;
using BikeBookingService.MessageQueue.Handlers;
using BikeRental.MessageQueue.MessageType;
using BikeRental.MessageQueue.SubscriptionManager;
using Hangfire;

namespace BikeBookingService.Extensions;

public static class WebApplicationExtension
{
    public static void RegisterMessageHandler(this WebApplication webApplication)
    {
        var serviceProvider = webApplication.Services;
        var messageQueueSubscriptionManager = serviceProvider.GetRequiredService<IMessageQueueSubscriptionManager>();
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeCreatedHandler>(
            MessageType.BikeCreated);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeUpdatedHandler>(
            MessageType.BikeUpdated);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeDeletedHandler>(
            MessageType.BikeDeleted);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeStationColorUpdatedHandler>(
            MessageType.BikeStationColorUpdated);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserCreatedEventHandler>(
            MessageType.UserAdded);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserDeletedEventHandler>(
            MessageType.UserDeleted);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserRoleUpdatedHandler>(
            MessageType.UserRoleUpdated);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<AccountPointLimitExceededHandler>(
            MessageType.AccountPointLimitExceeded);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<AccountPointSubtractedHandler>(
            MessageType.AccountPointSubtracted);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<AccountDebtHasBeenPaidHandler>(
            MessageType.AccountDebtHasBeenPaid);
    }

    public static void RegisterBackgroundJobs(this WebApplication webApplication)
    {
        var serviceProvider = webApplication.Services.CreateScope();
        RecurringJob.AddOrUpdate("JobId", () => 
            serviceProvider.ServiceProvider.GetRequiredService<IBikeTrackingBusinessLogic>()
                .CheckBikeRentingHasUserAlmostRunOutPoint(), Cron.Minutely);
    }
}
