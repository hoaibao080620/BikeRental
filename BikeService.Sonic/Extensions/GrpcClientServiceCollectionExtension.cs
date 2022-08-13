using Grpc.Net.Client.Web;

namespace BikeService.Sonic.Extensions;

public static class GrpcClientServiceCollectionExtension
{
    public static void RegisterGrpcClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddGrpcClient<BikeBookingServiceGrpc.BikeBookingServiceGrpcClient>("BikeBooking", c =>
        {
            c.Address = new Uri("https://bike-rental-booking-service.herokuapp.com");
        }).ConfigureChannel(o =>
        {
            o.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
        });
    }
}
