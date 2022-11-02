using System.Collections.Generic;
using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// Get Transactions Response Contract
    /// </summary>
    public class GetTransactionsResponseContractV1 : PlaidBaseResponseContractV1
    {
        /// <summary>
        /// Accounts
        /// </summary>
        [JsonProperty(PropertyName = "accounts")]
        public IEnumerable<PlaidAccountContractV1> Accounts { get; set; }

        /// <summary>
        /// Transactions
        /// </summary>
        [JsonProperty(PropertyName = "transactions")]
        public IEnumerable<PlaidTransactionContractV1> Transactions { get; set; }

        /// <summary>
        /// Item
        /// </summary>
        [JsonProperty(PropertyName = "item")]
        public PlaidItemContractV1 Item { get; set; }

        /// <summary>
        /// Total Transactions
        /// </summary>
        [JsonProperty(PropertyName = "total_transactions")]
        public uint TotalTransactions { get; set; }
    }
}
