using System;
using PirateKing.Core;
using PirateKing.Models;
using Newtonsoft.Json;

namespace PirateKing.Contracts.V1
{
    /// <summary>
    /// Put Transaction Request Contract
    /// </summary>
    public class PutTransactionRequestContractV1
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

        [JsonProperty(PropertyName = "verifiedDate")]
        public DateTime? VerifiedDate { get; set; }
    }
}
