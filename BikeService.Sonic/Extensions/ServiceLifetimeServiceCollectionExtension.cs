using BikeRental.MessageQueue.Consumer;
using BikeRental.MessageQueue.MessageType;
using BikeRental.MessageQueue.Publisher;
using BikeService.Sonic.BusinessLogics;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Services.Implementation;
using BikeService.Sonic.Services.Interfaces;
using BikeRental.MessageQueue.SubscriptionManager;
using BikeService.Sonic.Decorators;
using BikeService.Sonic.MessageQueue.Handlers;
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
        serviceCollection.AddScoped<IBikeBusinessLogic, BikeBusinessLogic>();
        serviceCollection.AddScoped<IImportService, BikeCsvImportService>();
        serviceCollection.AddScoped<IBikeStationBusinessLogic, BikeStationBusinessLogic>();
        serviceCollection.AddScoped<IBikeStationValidation, BikeStationValidation>();
        serviceCollection.AddScoped<ICacheService, RedisCacheService>();
        serviceCollection.AddScoped<IBikeLoaderAdapter, BikeLoaderConcrete>();
        serviceCollection.Decorate<IBikeLoaderAdapter>((inner, provider) => 
            new BikeLoaderAdapterWithCachingDecorator(
                inner, 
                provider.GetRequiredService<IDistributedCache>(),
                provider.GetRequiredService<ICacheService>()));
        
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        serviceCollection.AddScoped<IPublisher, SnsPublisher>();
        serviceCollection.AddScoped<IConsumer, SqsConsumer>();
        serviceCollection.AddScoped<IMessageQueuePublisher, MessageQueuePublisher>();
        serviceCollection.AddScoped<IBikeReportBusinessLogic, BikeReportBusinessLogic>();
    }
    
    public static void AddSingletonServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<IMessageQueueSubscriptionManager>(new MessageQueueSubscriptionManager());
        serviceCollection.AddSingleton<IGoogleMapService, GoogleMapService>();
    }

    public static void RegisterMessageHandlers(this IServiceCollection serviceCollection)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var messageQueueSubscriptionManager = serviceProvider.GetRequiredService<IMessageQueueSubscriptionManager>();
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserCreatedEventHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.UserAdded);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserDeletedEventHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.UserDeleted);
        
        messageQueueSubscriptionManager.RegisterEventHandlerSubscription<UserRoleUpdatedHandler>(
            serviceProvider.CreateScope().ServiceProvider, 
            MessageType.UserRoleUpdated);
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
