namespace PirateKing.Configurations
{
    /// <summary>
    /// Plaid Configuration
    /// </summary>
    public sealed class PlaidConfiguration
    {
        /// <summary>
        /// Client Configuration
        /// </summary>
        public ClientConfiguration ClientConfiguration { get; set;  }

        /// <summary>
        /// Access Token Prefix
        /// </summary>
        public string AccessTokenPrefix { get; set; }

        /// <summary>
        /// Plaid ClientId
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Plaid Secret
        /// </summary>
        public string Secret { get; set; }
    }
}
