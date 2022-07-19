using BikeRental.MessageQueue.Consumer;
using BikeRental.MessageQueue.SubscriptionManager;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using NotificationService.DAL;
using NotificationService.Hubs;

namespace NotificationService.Extensions;

public static class ServiceLifetimeServiceCollectionExtension 
{
    public static void AddScopedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<INotificationHub, NotificationHub>();
        serviceCollection.AddScoped<IConsumer, SqsConsumer>();
        serviceCollection.AddScoped<INotificationRepository, NotificationRepository>();
    }
    
    public static void AddSingletonServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMessageQueueSubscriptionManager>(new MessageQueueSubscriptionManager());
    }

    public static void AddMongoDb(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        BsonSerializer.RegisterSerializer(DateTimeSerializer.LocalInstance);
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:Database"];
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        serviceCollection.AddSingleton(database);
    }
}
