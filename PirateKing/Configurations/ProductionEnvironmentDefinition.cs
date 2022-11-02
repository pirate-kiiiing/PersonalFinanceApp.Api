namespace PirateKing.Configurations
{
    public class ProductionEnvironmentDefinition : EnvironmentDefinition
    {
        /// <summary>
        /// KeyVault Configuration
        /// </summary>
        public override ClientConfiguration KeyVaultConfiguration =>
            new ClientConfiguration
            {
                BaseAddress = "https://PirateKing.vault.azure.net",
            };

        /// <summary>
        /// Plaid Configuration
        /// </summary>
        public override PlaidConfiguration PlaidConfiguration =>
            new PlaidConfiguration
            {
                ClientConfiguration = new ClientConfiguration
                {
                    BaseAddress = "https://development.plaid.com",
                },
                AccessTokenPrefix = "access-development-",
            };
    }
}
