// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using id2;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            var tenantId = "37e55da6-fb62-456a-8d8e-f6f5b649092f";
            var clientId = "5be19e30-1ea9-4690-a37f-960d2190d4a3";

            services.AddAuthentication()
                .AddOpenIdConnect("oidc", "Azure Ad Demo", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.SaveTokens = true;

                    options.Authority = $"https://login.microsoftonline.com/{tenantId}";
                    options.ClientId = clientId;
                    options.ResponseType = OpenIdConnectResponseType.IdToken;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });

            var builder = services.AddIdentityServer(x =>
            {
                x.IssuerUri = "https://localhost:44334/idservice";
                
                //x.PublicOrigin = "https://localhost:44334/";
            })
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users)
                .AddProfileService<MyProfileService>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                context.Request.PathBase = new PathString("/idservice");
                return next();
            });

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders();

            app.UseStaticFiles();
            app.UseRouting();

            app.Use(async (context, next) =>
            {
                var headers = context.Request.Headers;
                foreach (var header in headers)
                {
                    System.Diagnostics.Debug.WriteLine(header.Key + "_" + header.Value);
                }

                await next.Invoke();
            });

            var options = new IdentityServerMiddlewareOptions();
            
            app.UseIdentityServer(new IdentityServerMiddlewareOptions()
            {
                
            }
                );

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }

    public class MyProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.IssuedClaims.Add(new Claim(IdentityModel.JwtClaimTypes.Role, "Administrator"));
            context.IssuedClaims.Add(new Claim(IdentityModel.JwtClaimTypes.Role, "User"));
            context.IssuedClaims.Add(new Claim(IdentityModel.JwtClaimTypes.Name, context.Subject.Identity.Name));

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}
