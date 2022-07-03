using MongoDB.Driver;
using UserService.BusinessLogic;
using UserService.Clients;
using UserService.DataAccess;
using UserService.ExternalServices;

namespace UserService.Extensions;

public static class ServiceLifetimeServiceCollectionExtension 
{
    public static void AddScopedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserBusinessLogic, UserBusinessLogic>();
        serviceCollection.AddAutoMapper(typeof(Program));
        serviceCollection.AddScoped<IOktaClient, OktaClient>();
        serviceCollection.AddScoped<IMessageQueuePublisher, MessageQueuePublisher>();
        serviceCollection.AddScoped<IMongoService, MongoService>();
    }
    
    public static void AddMongoDb(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:Database"];
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        serviceCollection.AddSingleton(database);
    }
}
