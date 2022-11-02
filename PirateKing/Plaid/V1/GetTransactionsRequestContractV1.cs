using System;
using System.Collections.Generic;
using PirateKing.Core;
using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// Get Transactions Request Contract
    /// </summary>
    public class GetTransactionsRequestContractV1
    {
        /// <summary>
        /// Access Token
        /// </summary>
        [JsonProperty(PropertyName = "access_token", Required = Required.Always)]
        public Guid AccessToken { get; set; }

        /// <summary>
        /// StartDate - Dates should be formatted as YYYY-MM-DD
        /// </summary>
        [JsonProperty(PropertyName = "start_date", Required = Required.Always)]
        public Date StartDate { get; set; }

        /// <summary>
        /// EndDate - Dates should be formatted as YYYY-MM-DD
        /// </summary>
        [JsonProperty(PropertyName = "end_date", Required = Required.Always)]
        public Date EndDate { get; set; }

        /// <summary>
        /// Options
        /// </summary>
        [JsonProperty(PropertyName = "options", NullValueHandling = NullValueHandling.Ignore)]
        public GetTransactionsOptionsRequestContractV1 Options { get; set; }
    }

    /// <summary>
    /// GetAccountBalanceOptions Request Contract
    /// </summary>
    public class GetTransactionsOptionsRequestContractV1
    {
        /// <summary>
        /// A list of account_ids to retrieve for the Item
        /// </summary>
        [JsonProperty(PropertyName = "account_ids")]
        public IEnumerable<string> AccountIds { get; set; }

        /// <summary>
        /// The number of transactions to fetch, where 0 < count <= 500.
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public uint Count { get; set; }

        /// <summary>
        /// The number of transactions to skip, where offset >= 0.
        /// </summary>
        [JsonProperty(PropertyName = "offset")]
        public uint Offset { get; set; }
    }
}
