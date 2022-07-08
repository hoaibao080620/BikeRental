﻿using BikeRental.MessageQueue.Consumer;
using BikeRental.MessageQueue.Publisher;
using BikeRental.MessageQueue.SubscriptionManager;
using BikeService.Sonic.DAL;
using BikeTrackingService.DAL;

namespace BikeTrackingService.Extensions;

public static class ServiceLifetimeServiceCollectionExtension 
{
    public static void AddScopedServices(this IServiceCollection serviceCollection)
    {
        // serviceCollection.AddScoped<ICacheService, RedisCacheService>();
        // serviceCollection.AddScoped<IBikeLoaderAdapter, BikeLoaderConcrete>();
        // serviceCollection.Decorate<IBikeLoaderAdapter>((inner, provider) => 
        //     new BikeLoaderAdapterWithCachingDecorator(
        //         inner, 
        //         provider.GetRequiredService<IDistributedCache>(),
        //         provider.GetRequiredService<ICacheService>()));
        //
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        serviceCollection.AddScoped<IPublisher, SnsPublisher>();
        serviceCollection.AddScoped<IConsumer, SqsConsumer>();
        // serviceCollection.AddScoped<IMessageQueuePublisher, MessageQueuePublisher>();
        // serviceCollection.AddScoped<IBikeReportBusinessLogic, BikeReportBusinessLogic>();
    }
    
    public static void AddSingletonServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMessageQueueSubscriptionManager>(new MessageQueueSubscriptionManager());
        // serviceCollection.AddSingleton<IGoogleMapService, GoogleMapService>();
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
}