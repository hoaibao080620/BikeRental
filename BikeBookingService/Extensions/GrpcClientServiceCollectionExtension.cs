using Grpc.Net.Client.Web;

namespace BikeBookingService.Extensions;

public static class GrpcClientServiceCollectionExtension
{
    public static void RegisterGrpcClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddGrpcClient<BikeServiceGrpc.BikeServiceGrpcClient>("BikeService", c =>
        {
            c.Address = new Uri("https://bike-service-13062022.herokuapp.com");
        }).ConfigureChannel(o =>
        {
            o.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
        });
    }
}
