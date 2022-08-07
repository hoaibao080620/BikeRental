using AccountService.BusinessLogic.Implementation;
using AccountService.BusinessLogic.Interfaces;
using AccountService.DataAccess;
using AccountService.MessageQueueHandlers;
using AccountService.Publisher;
using BikeRental.MessageQueue.Consumer;
using BikeRental.MessageQueue.MessageType;
using BikeRental.MessageQueue.Publisher;
using BikeRental.MessageQueue.SubscriptionManager;
using MongoDB.Driver;

namespace AccountService.Extensions;

public static class ServiceLifetimeServiceCollectionExtension 
{
    public static void AddScopedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(Program));
        serviceCollection.AddScoped<IConsumer, SqsConsumer>();
        serviceCollection.AddScoped<IPublisher, SnsPublisher>();
        serviceCollection.AddScoped<IMongoService, MongoService>();
        serviceCollection.AddScoped<IMessageQueuePublisher, MessageQueuePublisher>();
        serviceCollection.AddScoped<IAccountBusinessLogic, AccountBusinessLogic>();
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
            MessageType.UserAdded);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserUpdatedEventHandler>(
            MessageType.UserUpdated);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserDeletedEventHandler>(
            MessageType.UserDeleted);

        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<BikeCheckedOutEventHandler>(
            MessageType.BikeCheckedOut);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<PaymentPointSucceedEventHandler>(
            MessageType.PaymentSucceeded);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserDeactivatedEventHandler>(
            MessageType.AccountDeactivated);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserReactivatedEventHandler>(
            MessageType.AccountReactivated);
    }
    
    public static void AddMongoDb(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:Database"];
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        serviceCollection.AddSingleton(database);
    }
}
