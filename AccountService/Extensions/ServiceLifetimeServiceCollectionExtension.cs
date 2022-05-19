using AccountService.DataAccess.Implementation;
using AccountService.DataAccess.Interfaces;
using AccountService.MessageQueueHandlers;
using MessageQueue.Consumer;
using MessageQueue.Publisher;
using MessageQueue.SubscriptionManager;
using Shared.Consts;

namespace AccountService.Extensions;

public static class ServiceLifetimeServiceCollectionExtension 
{
    public static void AddScopedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        serviceCollection.AddAutoMapper(typeof(Program));
        serviceCollection.AddScoped<IConsumer, SqsConsumer>();
        serviceCollection.AddScoped<IPublisher, SqsPublisher>();
    }
    
    public static void AddSingletonServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMessageQueueSubscriptionManager>(new MessageQueueSubscriptionManager());
    }

    public static void RegisterMessageHandlers(this IServiceCollection serviceCollection)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var messageQueueSubscriptionManager = serviceProvider.GetRequiredService<IMessageQueueSubscriptionManager>();
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserCreatedEventHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.UserAdded);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserUpdatedEventHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.UserUpdated);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserDeletedEventHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.UserDeleted);
    }
}