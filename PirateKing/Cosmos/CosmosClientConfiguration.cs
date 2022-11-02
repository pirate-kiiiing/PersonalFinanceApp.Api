using System;
using System.Data.Common;
using PirateKing.Guards;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// Comsos Client Configuration
    /// </summary>
    public class CosmosClientConfiguration
    {
        private const string accountKeyName = "AccountKey";
        private const string accountEndpointName = "AccountEndpoint";

        /// <summary>
        /// The service endpoint used to create the client
        /// </summary>
        public Uri AccountEndpoint { get; }

        /// <summary>
        /// Account auth key for CosmosDB
        /// </summary>
        public string AccountKey { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public CosmosClientConfiguration(string connectionString)
        {
            Validate.NotNullOrEmpty(connectionString, nameof(connectionString));

            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString,
            };

            if (builder.TryGetValue(accountKeyName, out object key) == false)
            {
                throw new ArgumentException($"Missing {accountKeyName} from {nameof(connectionString)}");
            }
            if (builder.TryGetValue(accountEndpointName, out object uri) == false)
            {
                throw new ArgumentException($"Missing {accountEndpointName} from {nameof(connectionString)}");
            }

            this.AccountKey = key.ToString();
            this.AccountEndpoint = new Uri(uri.ToString());
        }
    }
}
