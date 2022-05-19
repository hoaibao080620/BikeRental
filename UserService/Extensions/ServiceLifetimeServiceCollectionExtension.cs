using UserService.BusinessLogic;
using UserService.Clients;
using UserService.DataAccess;
using UserService.ExternalServices;

namespace UserService.Extensions;

public static class ServiceLifetimeServiceCollectionExtension 
{
    public static void AddScopedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        serviceCollection.AddScoped<IUserBusinessLogic, UserBusinessLogic>();
        serviceCollection.AddAutoMapper(typeof(Program));
        serviceCollection.AddScoped<IOktaClient, OktaClient>();
        serviceCollection.AddScoped<IMessageQueuePublisher, MessageQueuePublisher>();
    }
}