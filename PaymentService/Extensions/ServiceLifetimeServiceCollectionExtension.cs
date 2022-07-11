using BikeRental.MessageQueue.Publisher;
using PaymentService.MessageQueue;
using PaymentService.Services;

namespace PaymentService.Extensions;

public static class ServiceLifetimeServiceCollectionExtension 
{
    public static void AddSingletonService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IPublisher, SnsPublisher>();
        serviceCollection.AddSingleton<IMessageQueuePublisher, MessageQueuePublisher>();
    }

    public static void AddScopeService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IStripeService, StripeService>();
    }
}
