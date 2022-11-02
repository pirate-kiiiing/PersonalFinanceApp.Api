using Autofac;
using PirateKing.Cosmos;
using PirateKing.KeyVault;

namespace PirateKing.Modules
{
    /// <summary>
    /// Cosmos Module
    /// </summary>
    public class CosmosModule : Module
    {
        /// <summary>
        /// Load CosmosDB Clients
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(CreateCosmosClient)
                .SingleInstance()
                .As<ICosmosClient>();

            builder.RegisterType<AccessTokenClient>()
                .SingleInstance()
                .As<IAccessTokenClient>();

            builder.RegisterType<AccountClient>()
                .SingleInstance()
                .As<IAccountClient>();

            builder.RegisterType<AccountCatalogClient>()
                .SingleInstance()
                .As<IAccountCatalogClient>();

            builder.RegisterType<GoogleUserClient>()
                .SingleInstance()
                .As<IGoogleUserClient>();

            builder.RegisterType<TenantClient>()
                .SingleInstance()
                .As<ITenantClient>();

            builder.RegisterType<TransactionClient>()
                .SingleInstance()
                .As<ITransactionClient>();

            builder.RegisterType<UserClient>()
                .SingleInstance()
                .As<IUserClient>();

            base.Load(builder);
        }

        private ICosmosClient CreateCosmosClient(IComponentContext componentContext)
        {
            string connectionString = KeyVaultClient.GetSecretAsync(SecretNames.AzureCosmosDb).Result;
            var configuration = new CosmosClientConfiguration(connectionString);

            return new CosmosClient(configuration);
        }
    }
}
