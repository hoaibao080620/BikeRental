using Grpc.Net.Client.Web;

namespace Aggregator.Extensions;

public static class GrpcClientServiceCollectionExtension
{
    public static void RegisterGrpcClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddGrpcClient<BikeServiceGrpc.BikeServiceGrpcClient>("BikeService", c =>
        {
            c.Address = new Uri("https://localhost:7199");
        }).ConfigureChannel(o =>
        {
            o.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
        });
        
        serviceCollection.AddGrpcClient<AccountServiceGrpc.AccountServiceGrpcClient>("AccountService", c =>
        {
            c.Address = new Uri("https://localhost:7181");
        }).ConfigureChannel(o =>
        {
            o.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
        });
        
        serviceCollection.AddGrpcClient<BikeBookingServiceGrpc.BikeBookingServiceGrpcClient>("BikeBookingService", c =>
        {
            c.Address = new Uri("https://localhost:7230");
        }).ConfigureChannel(o =>
        {
            o.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
        });
    }
}
