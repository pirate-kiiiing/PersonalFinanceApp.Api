using System;
using PirateKing.Core;
using Newtonsoft.Json;

namespace PirateKing.Contracts.V1
{
    /// <summary>
    /// Put Account Catalog Request Contract
    /// </summary>
    public class PutAccountCatalogRequestContractV1
    {
        /// <summary>
        /// Account Identifier
        /// </summary>
        [JsonProperty(PropertyName = "accountId")]
        public Guid AccountId { get; set; }

        /// <summary>
        /// Date in yyyy-MM-dd format
        /// </summary>
        [JsonProperty(PropertyName = "date")]
        public Date Date { get; set; }

        /// <summary>
        /// Value in the account
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }
    }
}
