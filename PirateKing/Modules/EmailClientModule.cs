using Autofac;
using PirateKing.Email;

namespace PirateKing.Modules
{
    internal class EmailClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OutlookEmailClient>()
                .SingleInstance()
                .As<IEmailClient>();

            base.Load(builder);
        }
    }
}
