using System;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace UITester
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            AddMicroserviceClients(services);

            // Without this the claim handling has issues (taken from IdentityServer's examples)
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            // This configuration is used when the user clicks the Privacy-link. This isn't used when the HttpClients communicate with the Microservices.
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://localhost:44334/idservice";
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "mvc";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";
                    options.SaveTokens = true;

                    // We want the user to be able to call all the Microservices.
                    options.Scope.Add("microservice1");
                    options.Scope.Add("microservice2");
                    options.Scope.Add("supermicroservice");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });
        }

        private static void AddMicroserviceClients(IServiceCollection services)
        {
            // Sets the address of the IdentityServer and the ClientID & Secret. 
            // These are used with all the HttpClients registered below.
            services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add("identityserver", new TokenClientOptions
                {
                    Address = "https://localhost:44334/idservice/connect/token",
                    ClientId = "mymvcclient",
                    ClientSecret = "secret"
                });
            });

            services.AddClientAccessTokenClient("microservice1", configureClient: client =>
            {
                client.BaseAddress = new Uri("https://localhost:44334/api/v1/microservice1/");
            });

            services.AddClientAccessTokenClient("microservice2", configureClient: client =>
            {
                client.BaseAddress = new Uri("https://localhost:44334/api/v1/microservice2/");
            });

            services.AddClientAccessTokenClient("supermicroservice", configureClient: client =>
            {
                client.BaseAddress = new Uri("https://localhost:44334/api/v1/supermicroservice/");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
