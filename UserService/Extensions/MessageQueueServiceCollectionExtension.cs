using BikeRental.MessageQueue;
using BikeRental.MessageQueue.Consumer;
using BikeRental.MessageQueue.Publisher;

namespace UserService.Extensions;

public static class MessageQueueServiceCollectionExtension
{
    public static void AddMessageQueueServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IPublisher, SqsPublisher>();
        serviceCollection.AddScoped<IConsumer, SqsConsumer>();
    }
}