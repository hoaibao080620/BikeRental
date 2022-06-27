using BikeRental.MessageQueue.Consumer;
using BikeRental.MessageQueue.Publisher;
using BikeRental.MessageQueue.SubscriptionManager;

namespace AccountService.Extensions;

public static class ServiceLifetimeServiceCollectionExtension 
{
    public static void AddScopedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(Program));
        serviceCollection.AddScoped<IConsumer, SqsConsumer>();
        serviceCollection.AddScoped<IPublisher, SqsPublisher>();
    }
    
    public static void AddSingletonServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMessageQueueSubscriptionManager>(new MessageQueueSubscriptionManager());
    }

    // public static void RegisterMessageHandlers(this IServiceCollection serviceCollection)
    // {
    //     var serviceProvider = serviceCollection.BuildServiceProvider();
    //     var messageQueueSubscriptionManager = serviceProvider.GetRequiredService<IMessageQueueSubscriptionManager>();
    //     
    //     messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserCreatedEventHandler>(
    //         serviceProvider.CreateScope().ServiceProvider, 
    //         MessageType.UserAdded);
    //     
    //     messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserUpdatedEventHandler>(
    //         serviceProvider.CreateScope().ServiceProvider, 
    //         MessageType.UserUpdated);
    //     
    //     messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserDeletedEventHandler>(
    //         serviceProvider.CreateScope().ServiceProvider, 
    //         MessageType.UserDeleted);
    // }
}
