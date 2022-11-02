using Autofac;
using AzureFunctions.Autofac.Configuration;
using PirateKing.Configurations;
using PirateKing.KeyVault;
using PirateKing.Modules;

namespace PirateKing.Functions
{
    public class AutofacConfig
    {
        public AutofacConfig(string functionName)
        {
            var environment = EnvironmentDefinition.GetEnvironmentDefinition();

            DependencyInjection.Initialize(builder =>
            {
                // Autofac Registrations
                builder.RegisterModule(new CosmosModule());
                builder.RegisterModule(new PirateKingEmailClientModule());
                builder.RegisterModule(new PlaidClientModule(environment.PlaidConfiguration));
            }, functionName);

            // needed for getting email secrets
            KeyVaultClient.Init(environment.KeyVaultConfiguration);
        }
    }
}
