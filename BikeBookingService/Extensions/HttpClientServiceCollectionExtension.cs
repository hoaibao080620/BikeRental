using BikeBookingService.Controllers;
using BikeBookingService.Services;
using BikeService.Sonic.Services.Implementation;

namespace BikeBookingService.Extensions;

public static class HttpClientServiceCollectionExtension
{
    public static void AddHttpClientToServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IGoogleMapService, GoogleMapService>();
        serviceCollection.AddHttpClient<BikeTrackingController>();
    }
}
