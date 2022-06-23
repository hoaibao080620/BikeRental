using AccountService.AccountDbContext;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Extensions;

public static class DbContextServiceCollectionExtension
{
    public static void AddDbContextService(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<AccountServiceDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("MysqlServer");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
    }
    
    public static void RunMigrations(this IServiceCollection serviceCollection)
    {
        using var scope = serviceCollection.BuildServiceProvider().CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
        db.Database.Migrate();
    }
}
