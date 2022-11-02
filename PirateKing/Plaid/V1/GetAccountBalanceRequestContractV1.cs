using System.Collections.Generic;
using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// GetAccountBalance Request Contract
    /// </summary>
    public class GetAccountBalanceRequestContractV1
    {
        /// <summary>
        /// Access Token
        /// </summary>
        [JsonProperty(PropertyName = "access_token", Required = Required.Always)]
        public string AccessToken { get; set; }

        /// <summary>
        /// Options
        /// </summary>
        [JsonProperty(PropertyName = "options", NullValueHandling = NullValueHandling.Ignore)]
        public GetAccountBalanceOptionsRequestContractV1 Options { get; set; }
    }

    /// <summary>
    /// GetAccountBalanceOptions Request Contract
    /// </summary>
    public class GetAccountBalanceOptionsRequestContractV1
    {
        /// <summary>
        /// A list of account_ids to retrieve for the Item
        /// </summary>
        [JsonProperty(PropertyName = "account_ids", Required = Required.Default)]
        public IEnumerable<string> AccountIds { get; set; }
    }
}
