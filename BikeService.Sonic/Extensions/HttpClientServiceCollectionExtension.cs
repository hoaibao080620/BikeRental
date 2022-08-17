using BikeService.Sonic.GrpcServices;
using BikeService.Sonic.Services.Implementation;
using BikeService.Sonic.Services.Interfaces;

namespace BikeService.Sonic.Extensions;

public static class HttpClientServiceCollectionExtension
{
    public static void AddHttpClientToServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IGoogleMapService, GoogleMapService>();
    }
}
