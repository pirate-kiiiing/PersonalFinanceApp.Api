using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// Plaid Account Contract
    /// </summary>
    public class PlaidAccountContractV1
    {
        [JsonProperty(PropertyName = "account_id", Required = Required.Always)]
        public string AccountId { get; set; }

        [JsonProperty(PropertyName = "balances")]
        public PlaidBalanceContractV1 Balance { get; set; }

        [JsonProperty(PropertyName = "mask")]
        public string Mask { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "official_name")]
        public string OfficialName { get; set; }

        [JsonProperty(PropertyName = "subtype")]
        public string Subtype { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}
