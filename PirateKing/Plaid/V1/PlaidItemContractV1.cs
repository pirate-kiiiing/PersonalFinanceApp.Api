using System.Collections.Generic;
using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// Plaid Item Contract
    /// </summary>
    public class PlaidItemContractV1
    {
        [JsonProperty(PropertyName = "item_id")]
        public string ItemId { get; set; }

        [JsonProperty(PropertyName = "institution_id")]
        public string InstitutionId { get; set; }

        [JsonProperty(PropertyName = "available_products")]
        public IEnumerable<string> AvailableProducts { get; set; }

        [JsonProperty(PropertyName = "billed_products")]
        public IEnumerable<string> BilledProducts { get; set; }

        [JsonProperty(PropertyName = "webhook")]
        public string Webhook { get; set; }

        [JsonProperty(PropertyName = "error")]
        public PlaidErrorResponseContractV1 Error { get; set; }
    }
}
