// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                // Does the gateway need its own scope? Disabled in this example but in some scenarios this could be useful. 
                // If this is enabled, the authentication on Ocelot could be handled using the gateway scope
                // new ApiResource("gateway", "My Gateway"),
                new ApiResource("supermicroservice", "My Super Microservice"),
                new ApiResource("microservice1", "My First Microservice"),
                new ApiResource("microservice2", "My Second Microservice"),
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // Client which can access all the microservices
                new Client
                {
                    ClientId = "mysupermicroserviceclient",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "microservice1", "microservice2" }
                    // If gateway scope is used, enable the following instead. Make sure to add "gateway" to other clients too
                    //AllowedScopes = { "gateway", "microservice1", "microservice2" }
                },
                // Another client which can access all the microservices
                new Client
                {
                    ClientId = "mymvcclient",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "microservice1", "microservice2" }
                },
                // Client which can only access microservice2
                new Client
                {
                    ClientId = "mymicroservice1client",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "microservice2" }
                },
                // Client which can only access microservice
                new Client
                {
                    ClientId = "mymicroservice2client",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "microservice1" }
                },
                // Client which supports UI based sign-in and can access all the microservices
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,

                    RedirectUris = { "https://localhost:44322/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:44322/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "microservice1",
                        "microservice2",
                        "supermicroservice",
                    },

                    AlwaysIncludeUserClaimsInIdToken = true,
                }
        };
    }
}
