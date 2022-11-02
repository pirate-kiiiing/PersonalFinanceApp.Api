using System;
using PirateKing.Models;
using Newtonsoft.Json;

namespace PirateKing.Contracts.V1
{
    /// <summary>
    /// Get Account Response Contract
    /// </summary>
    public class GetAccountResponseContractV1
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "symbol")]
        public string Symbol { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tenantId")]
        public Guid TenantId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        [JsonProperty(PropertyName = "isTracked")]
        public bool IsTracked { get; set; }

        [JsonProperty(PropertyName = "state")]
        public AccountState State { get; set; }

        [JsonProperty(PropertyName = "assetType", NullValueHandling = NullValueHandling.Ignore)]
        public AssetType? AssetType { get; set; }

        [JsonProperty(PropertyName = "expenseType", NullValueHandling = NullValueHandling.Ignore)]
        public ExpenseType? ExpenseType { get; set; }
    }
}
