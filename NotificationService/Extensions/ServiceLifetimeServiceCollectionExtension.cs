using BikeRental.MessageQueue.Consumer;
using BikeRental.MessageQueue.MessageType;
using BikeRental.MessageQueue.SubscriptionManager;
using NotificationService.Hub;
using NotificationService.MessageQueue.MessageQueueHandlers;

namespace NotificationService.Extensions;

public static class ServiceLifetimeServiceCollectionExtension 
{
    public static void AddScopedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBikeLocationHub, BikeLocationHub>();
        serviceCollection.AddScoped<IConsumer, SqsConsumer>();
    }
    
    public static void AddSingletonServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMessageQueueSubscriptionManager>(new MessageQueueSubscriptionManager());
    }

    public static void RegisterMessageHandlers(this IServiceCollection serviceCollection)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();
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
