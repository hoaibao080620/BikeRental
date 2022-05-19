using AccountService.AccountDbContext;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Extensions;

public static class DbContextServiceCollectionExtension
{
    public static void AddDbContextService(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<AccountServiceDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
        });
    }
}