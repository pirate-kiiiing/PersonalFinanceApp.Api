using System.Threading.Tasks;
using Autofac;
using PirateKing.Configurations;
using PirateKing.Core;
using PirateKing.KeyVault;
using PirateKing.Models;
using PirateKing.Modules;
using PirateKing.Tokens;
using PirateKing.Api.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace PirateKing.Api
{
    public class Startup
    {
        private const string apiCorsPolicy = nameof(apiCorsPolicy);
        private static string[] allowedMethods = new string[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy(apiCorsPolicy, builder =>
            {
                builder.WithOrigins("http://localhost:3030").WithMethods(allowedMethods).AllowAnyHeader().AllowCredentials();
                builder.WithExposedHeaders(HeaderNames.ETag);
            }));
            
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = JwtToken.GetTokenValidationParameters();
                });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            EnvironmentDefinition environment = EnvironmentDefinition.GetEnvironmentDefinition();

            // initializes classes that provides static interfaces
            KeyVaultClient.Init(environment.KeyVaultConfiguration);

            // start async registrations
            Task configureTasks = ConfigureContainerAsync(environment);

            // Type Registrations
            builder.RegisterType<DependencyFactory>()
                .As<IDependencyFactory>()
                .SingleInstance();

            // Module registrations
            builder.RegisterModule(new CloudQueueClientModule());
            builder.RegisterModule(new CosmosModule());
            builder.RegisterModule(new PlaidClientModule(environment.PlaidConfiguration));

            // wait for async registrations to complete
            configureTasks.Wait();
        }

        private async Task ConfigureContainerAsync(EnvironmentDefinition environment)
        {
            // start tasks
            Task<string> getJwtSecretsTask = KeyVaultClient.GetSecretAsync(SecretNames.JwtSecrets);

            // wait for completion
            string jwtSecretsString = await getJwtSecretsTask;
            var jwtSecrets = jwtSecretsString.Deserialize<JwtSecrets>();

            // configure
            JwtToken.SetJwtSecrets(jwtSecrets);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(apiCorsPolicy);
            app.UseAuthentication();
            app.UseMiddleware(typeof(UnhandledExceptionHandler));
            app.UseMvc();
        }
    }
}
