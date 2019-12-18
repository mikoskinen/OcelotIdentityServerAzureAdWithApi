using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Api
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
            services.AddControllers();

            // Create the clients which are used to call other microservices
            services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add("identityserver", new TokenClientOptions
                {
                    Address = "https://localhost:44334/idservice/connect/token",
                    ClientId = "mymicroservice2client",
                    ClientSecret = "secret"
                });
            });

            services.AddClientAccessTokenClient("microservice1", configureClient: client =>
            {
                client.BaseAddress = new Uri("https://localhost:44334/api/v1/microservice1/");
            });

            services.AddClientAccessTokenClient("supermicroservice", configureClient: client =>
            {
                client.BaseAddress = new Uri("https://localhost:44334/api/v1/supermicroservice/");
            });

            // Identifies this microservice
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:44334/idservice";
                    options.Audience = "microservice2";
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
