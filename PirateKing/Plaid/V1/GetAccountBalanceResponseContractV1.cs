using System.Collections.Generic;
using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// GetAccountBalance Response Contract
    /// </summary>
    public class GetAccountBalanceResponseContractV1 : PlaidBaseResponseContractV1
    {
        [JsonProperty(PropertyName = "accounts", Required = Required.Always)]
        public IEnumerable<PlaidAccountContractV1> Accounts { get; set; }

        [JsonProperty(PropertyName = "item")]
        public PlaidItemContractV1 Item { get; set; }
    }
}
