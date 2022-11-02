using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// Plaid  Balance Contract
    /// </summary>
    public class PlaidBalanceContractV1
    {
        [JsonProperty(PropertyName = "available")]
        public decimal? Available { get; set; }

        [JsonProperty(PropertyName = "current")]
        public decimal Current { get; set; }

        [JsonProperty(PropertyName = "iso_currency_code")]
        public string IsoCurrencyCode { get; set; }

        [JsonProperty(PropertyName = "limit")]
        public decimal? Limit { get; set; }

        [JsonProperty(PropertyName = "unofficial_currency_code")]
        public string UnofficialCurrencyCode { get; set; }
    }
}
