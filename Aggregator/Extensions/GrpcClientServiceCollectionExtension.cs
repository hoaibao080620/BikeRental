﻿using Grpc.Net.Client.Web;

namespace Aggregator.Extensions;

public static class GrpcClientServiceCollectionExtension
{
    public static void RegisterGrpcClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddGrpcClient<BikeServiceGrpc.BikeServiceGrpcClient>("BikeService", c =>
        {
            c.Address = new Uri("https://bike-service-13062022-1.herokuapp.com");
        }).ConfigureChannel(o =>
        {
            o.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
        });
        
        serviceCollection.AddGrpcClient<AccountServiceGrpc.AccountServiceGrpcClient>("AccountService", c =>
        {
            c.Address = new Uri("https://bike-rental-account-service-1.herokuapp.com");
        }).ConfigureChannel(o =>
        {
            o.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
        });
        
        serviceCollection.AddGrpcClient<BikeBookingServiceGrpc.BikeBookingServiceGrpcClient>("BikeBookingService", c =>
        {
            c.Address = new Uri("https://bike-rental-booking-service-1.herokuapp.com");
        }).ConfigureChannel(o =>
        {
            o.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
        });
        
        serviceCollection.AddGrpcClient<NotificationServiceGrpc.NotificationServiceGrpcClient>("NotificationService", c =>
        {
            c.Address = new Uri("https://bike-rental-notification-1.herokuapp.com");
        }).ConfigureChannel(o =>
        {
            o.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
        });
    }
}
