using Autofac;
using PirateKing.CloudQueue;
using PirateKing.KeyVault;

namespace PirateKing.Modules
{
    public class CloudQueueClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(CreateCloudQueueClient)
                .SingleInstance()
                .As<ICloudQueueClient>();

            base.Load(builder);
        }

        private CloudQueueClient CreateCloudQueueClient(IComponentContext componentContext)
        {
            string storageConnectionString = KeyVaultClient.GetSecretAsync(SecretNames.StorageConnectionString).Result;

            return new CloudQueueClient(storageConnectionString);
        }
    }
}