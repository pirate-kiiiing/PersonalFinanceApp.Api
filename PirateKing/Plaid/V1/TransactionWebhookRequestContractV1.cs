using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PirateKing.Plaid.V1
{
    public class TransactionWebhookRequestContractV1
    {
        /// <summary>
        /// Webhook Type
        /// </summary>
        [JsonProperty(PropertyName = "webhook_type", Required = Required.Always)]
        public PlaidWebhookType WebhookType { get; set; }

        /// <summary>
        /// Webhook Code
        /// </summary>
        [JsonProperty(PropertyName = "webhook_code", Required = Required.Always)]
        public TransactionWebhookCode WebhookCode { get; set; }

        /// <summary>
        /// Item Identifier
        /// </summary>
        [JsonProperty(PropertyName = "item_id", Required = Required.Always)]
        public string ItemId { get; set; }

        /// <summary>
        /// New Transactions
        /// </summary>
        [JsonProperty(PropertyName = "new_transactions")]
        public short? NewTransactions { get; set; }

        /// <summary>
        /// Removed Transactions
        /// </summary>
        [JsonProperty(PropertyName = "removed_transactions")]
        public string[] RemovedTransactions { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }

    /// <summary>
    /// Account Tracking Type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionWebhookCode
    {
        /// <summary>
        /// Fired when an Item's initial transaction pull is completed.
        /// Note: The default pull is 30 days.
        /// </summary>
        INITIAL_UPDATE,

        /// <summary>
        /// Fired when an Item's historical transaction pull is completed. 
        /// Plaid fetches as much data as is available from the financial institution.
        /// </summary>
        HISTORICAL_UPDATE,

        /// <summary>
        /// Fired when new transaction data is available as Plaid performs its regular updates of the Item.
        /// </summary>
        DEFAULT_UPDATE,

        /// <summary>
        /// Fired when posted transaction(s) for an Item are deleted. 
        /// The deleted transaction IDs are included in the webhook payload.
        /// </summary>
        TRANSACTIONS_REMOVED,
    }
}
