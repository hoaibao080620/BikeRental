using BikeService.Sonic.Services.Implementation;
using BikeTrackingService.Services;

namespace BikeTrackingService.Extensions;

public static class HttpClientServiceCollectionExtension
{
    public static void AddHttpClientToServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IGoogleMapService, GoogleMapService>();
    }
}
