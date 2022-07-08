using BikeTrackingService.BikeTrackingServiceDbContext;
using Microsoft.EntityFrameworkCore;

namespace BikeTrackingService.Extensions;

public static class DbContextServiceCollectionExtension
{
    public static void AddDbContextService(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<BikeTrackingDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Postgres");
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });
    }
    
    public static void RunMigrations(this IServiceCollection serviceCollection)
    {
        using var scope = serviceCollection.BuildServiceProvider().CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BikeTrackingDbContext>();
        db.Database.Migrate();
    }
}
