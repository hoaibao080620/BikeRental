using BikeRental.MessageQueue.Consumer;
using BikeRental.MessageQueue.SubscriptionManager;
using NotificationService.Hubs;

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
}
