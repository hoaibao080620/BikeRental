using Microsoft.EntityFrameworkCore;
using UserService.ApplicationDbContext;

namespace UserService.Extensions;

public static class DbContextServiceCollectionExtension
{
    public static void AddDbContextService(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<UserServiceDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
        });
    }
}