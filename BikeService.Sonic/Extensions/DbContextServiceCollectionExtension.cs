using BikeService.Sonic.BikeDbContext;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.Extensions;

public static class DbContextServiceCollectionExtension
{
    public static void AddDbContextService(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<BikeServiceDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
        });
    }
}