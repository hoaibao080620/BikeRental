namespace BikeBookingService.Extensions;

public static class CachingServiceCollectionExtension
{
    public static void AddRedisCache(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddStackExchangeRedisCache(options =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            options.Configuration = connectionString;
        });
    }
}
