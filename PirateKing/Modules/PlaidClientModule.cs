using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using PirateKing.Configurations;
using PirateKing.Guards;
using PirateKing.KeyVault;
using PirateKing.Plaid.V1;

namespace PirateKing.Modules
{
    /// <summary>
    /// Plaid Client Module
    /// </summary>
    public class PlaidClientModule : Module
    {
        private readonly PlaidConfiguration configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public PlaidClientModule(PlaidConfiguration configuration)
        {
            Validate.NotNull(configuration, nameof(configuration));

            this.configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(CreatePlaidClientV1)
                .SingleInstance()
                .As<IPlaidClientV1>();

            base.Load(builder);
        }

        private IPlaidClientV1 CreatePlaidClientV1(IComponentContext componentContext)
        {
            Task<string> clientIdTask = KeyVaultClient.GetSecretAsync(SecretNames.PlaidClientId);
            Task<string> secretTask = KeyVaultClient.GetSecretAsync(SecretNames.PlaidSecret);

            Task.WaitAll(clientIdTask, secretTask);

            string clientId = clientIdTask.Result;
            string secret = secretTask.Result;

            configuration.ClientId = clientId;
            configuration.Secret = secret;

            var handler = new PlaidClientHandlerV1(configuration);
            var clientConfiguration = configuration.ClientConfiguration;
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(clientConfiguration.BaseAddress),
            };

            return new PlaidClientV1(httpClient, configuration);
        }
    }
}
