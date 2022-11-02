using Autofac;
using PirateKing.Email;
using PirateKing.KeyVault;
using PirateKing.Models;

namespace PirateKing.Modules
{
    public class PirateKingEmailClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new EmailClientModule());

            builder.Register(Create)
                .SingleInstance()
                .As<IPirateKingEmailClient>();

            base.Load(builder);
        }

        private IPirateKingEmailClient Create(IComponentContext componentContext)
        {
            IEmailClient emailClient = componentContext.Resolve<IEmailClient>();
            EmailSecrets emailSecrets = KeyVaultClient.GetEmailSecretsAsync().Result;

            return new PirateKingEmailClient(emailClient, emailSecrets);
        }
    }
}
