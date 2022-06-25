﻿using BikeService.Sonic.BusinessLogics;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Services.Implementation;
using BikeService.Sonic.Services.Interfaces;
using BikeRental.MessageQueue.SubscriptionManager;
using BikeService.Sonic.Decorators;
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
        serviceCollection.AddScoped<IBikeLocationTrackingRepository, BikeLocationTrackingRepository>();
        serviceCollection.AddScoped<IAccountRepository, AccountRepository>();
        serviceCollection.AddScoped<IBikeStationBusinessLogic, BikeStationBusinessLogic>();
        serviceCollection.AddScoped<IBikeRentalTrackingHistoryRepository, BikeRentalTrackingHistoryRepository>();
        serviceCollection.AddScoped<IBikeStationValidation, BikeStationValidation>();
        serviceCollection.AddScoped<IBikeRepository, BikeRepository>();

        serviceCollection.AddScoped<IBikeRepositoryAdapter, BikeRepositoryConcrete>();
        serviceCollection.Decorate<IBikeRepositoryAdapter>((inner, provider) => 
            new BikeRepositoryAdapterWithCachingDecorator(
                inner, 
                provider.GetRequiredService<IDistributedCache>(),
                provider.GetRequiredService<IGoogleMapService>()));
    }
    
    public static void AddSingletonServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        // serviceCollection.AddStackExchangeRedisCache(options =>
        // {
        //     options.Configuration = configuration["Redis:Url"];
        // });
        
        // serviceCollection.AddSingleton(new MongoClient(configuration["MongoDB:ConnectionString"])
        //     .GetDatabase(configuration["MongoDB:Database"]));

        // serviceCollection.AddSingleton<IBikeStationRepository, BikeStationRepository>();
        // serviceCollection.AddSingleton<IBikeRepository, BikeRepository>();
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
