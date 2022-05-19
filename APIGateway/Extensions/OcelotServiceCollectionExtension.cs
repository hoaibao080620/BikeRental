using System.IdentityModel.Tokens.Jwt;
using Ocelot.DependencyInjection;

namespace WebBFFGateway.Extensions;

public static class OcelotServiceCollectionExtension
{
    public static void AddOcelotConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        IConfiguration ocelotConfiguration = new ConfigurationBuilder()
            .AddJsonFile("ocelot.json")
            .Build();

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("scp");
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add("scp", "scope");
        const string oktaProviderKey = "OktaProvider";
        serviceCollection
            .AddAuthentication()
            .AddJwtBearer(oktaProviderKey, options =>
            {
                options.Audience = configuration["Okta:Audience"];
                options.Authority = configuration["Okta:Issuer"];
            });
        serviceCollection.AddOcelot(ocelotConfiguration);
    }
}