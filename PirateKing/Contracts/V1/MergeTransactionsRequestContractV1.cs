using System;
using Newtonsoft.Json;

namespace PirateKing.Contracts.V1
{
    /// <summary>
    /// Merge Transactions Request Contract
    /// </summary>
    public class MergeTransactionsRequestContractV1
    {
        /// <summary>
        /// Pending transaction identifer to be merged with a settled transaction
        /// </summary>
        [JsonProperty(PropertyName = "pendingTransactionId", Required = Required.Always)]
        public Guid PendingTransactionId { get; set; }

        /// <summary>
        /// Verified Date
        /// </summary>
        [JsonProperty(PropertyName = "verifiedDate")]
        public DateTime? VerifiedDate { get; set; }
    }
}
