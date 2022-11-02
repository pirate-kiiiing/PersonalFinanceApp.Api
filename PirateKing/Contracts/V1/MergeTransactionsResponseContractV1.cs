using System;
using PirateKing.Core;
using PirateKing.Models;
using Newtonsoft.Json;

namespace PirateKing.Contracts.V1
{
    /// <summary>
    /// Merge Transaction Response Contract
    /// </summary>
    public class MergeTransactionsResponseContractV1
    {
        [JsonProperty(PropertyName = "id")]
        public Guid TransactionId { get; set; }

        [JsonProperty(PropertyName = "date")]
        public Date Date { get; set; }

        [JsonProperty(PropertyName = "tenantId")]
        public Guid TenantId { get; set; }

        [JsonProperty(PropertyName = "accountId")]
        public Guid AccountId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "expenseCategory")]
        public ExpenseCategory ExpenseCategory { get; set; }

        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }

        [JsonProperty(PropertyName = "isPending")]
        public bool IsPending { get; set; }

        [JsonProperty(PropertyName = "mergedDate")]
        public DateTime? MergedDate { get; set; }

        [JsonProperty(PropertyName = "verifiedDate")]
        public DateTime? VerifiedDate { get; set; }
    }
}
