﻿using System.IdentityModel.Tokens.Jwt;
using Okta.AspNetCore;
using UserService.BusinessLogic;

namespace UserService.Extensions;

public static class OktaServiceCollectionExtension
{
    public static void AddOktaAuthenticationService(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("scp");
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add("scp", "scope");
        serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
                options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
                options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
            })
            .AddOktaWebApi(new OktaWebApiOptions
            {
                OktaDomain = configuration.GetValue<string>("Okta:Domain"),
                AuthorizationServerId = configuration.GetValue<string>("Okta:AuthorizationServerId"),
                Audience = configuration.GetValue<string>("Okta:Audience")
            });

        serviceCollection.AddAuthorization();
    }

    public static async Task SyncOktaUsers(this IServiceCollection serviceCollection)
    {
        var userBusinessLogic = serviceCollection.BuildServiceProvider().GetRequiredService<IUserBusinessLogic>();
        await userBusinessLogic.SyncOktaUsers();
    }
}