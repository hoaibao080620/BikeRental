using BikeRental.MessageQueue.Consumer;
using BikeRental.MessageQueue.Publisher;
using BikeService.Sonic.BusinessLogics;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Services.Implementation;
using BikeService.Sonic.Services.Interfaces;
using BikeRental.MessageQueue.SubscriptionManager;
using BikeService.Sonic.Decorators;
using BikeService.Sonic.MessageQueue.Publisher;
using BikeService.Sonic.Validation;
using Microsoft.Extensions.Caching.Distributed;
using Nest;
using Shared.Service;

namespace BikeService.Sonic.Extensions;

public static class ServiceLifetimeServiceCollectionExtension 
{
    public static void AddScopedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IElasticSearchService, ElasticSearchService>();
        serviceCollection.AddScoped<IBikeLocationHub, BikeLocationHub>();
        serviceCollection.AddScoped<IBikeStationManagerRepository, BikeStationManagerRepository>();
        serviceCollection.AddScoped<IBikeBusinessLogic, BikeBusinessLogic>();
        serviceCollection.AddScoped<IImportService, BikeCsvImportService>();
        serviceCollection.AddScoped<IBikeStationRepository, BikeStationRepository>();
        serviceCollection.AddScoped<IBikeStationBusinessLogic, BikeStationBusinessLogic>();
        serviceCollection.AddScoped<IBikeStationValidation, BikeStationValidation>();

        serviceCollection.AddScoped<IBikeRepositoryAdapter, BikeRepositoryConcrete>();
        serviceCollection.Decorate<IBikeRepositoryAdapter>((inner, provider) => 
            new BikeRepositoryAdapterWithCachingDecorator(
                inner, 
                provider.GetRequiredService<IDistributedCache>(),
                provider.GetRequiredService<IGoogleMapService>()));

        serviceCollection.AddScoped<ICacheService, RedisCacheService>();
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        serviceCollection.AddScoped<IPublisher, SqsPublisher>();
        serviceCollection.AddScoped<IConsumer, SqsConsumer>();
        serviceCollection.AddScoped<IMessageQueuePublisher, MessageQueuePublisher>();
    }
    
    public static void AddSingletonServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<IMessageQueueSubscriptionManager>(new MessageQueueSubscriptionManager());
        serviceCollection.AddSingleton<IGoogleMapService, GoogleMapService>();
    }

    public static void RegisterMessageHandlers(this IServiceCollection serviceCollection)
    {
        // var serviceProvider = serviceCollection.BuildServiceProvider();
        // var messageQueueSubscriptionManager = serviceProvider.GetRequiredService<IMessageQueueSubscriptionManager>();
        //
        // messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserCreatedEventHandler>(
        //     serviceProvider.CreateScope().ServiceProvider, 
        //     MessageType.UserAdded);
        //
        // messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserUpdatedEventHandler>(
        //     serviceProvider.CreateScope().ServiceProvider, 
        //     MessageType.UserUpdated);
        //
        // messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserDeletedEventHandler>(
        //     serviceProvider.CreateScope().ServiceProvider, 
        //     MessageType.UserDeleted);
    }
    
    public static void AddElasticClient(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
            .DefaultIndex(configuration["ElasticSearch:BikeStationIndex"]);
        
        settings.EnableApiVersioningHeader();
        var client = new ElasticClient(settings);
        serviceCollection.AddSingleton<IElasticClient>(client);
    } 
}
