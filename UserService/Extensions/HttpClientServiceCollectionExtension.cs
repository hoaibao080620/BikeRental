using UserService.Clients;

namespace UserService.Extensions;

public static class HttpClientServiceCollectionExtension
{
    public static void AddHttpClientToServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IOktaClient, OktaClient>();
    }
}