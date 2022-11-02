using System.Collections.Generic;
using PirateKing.Core;
using PirateKing.Models;
using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// Plaid Transaction Contract
    /// </summary>
    public class PlaidTransactionContractV1
    {
        /// <summary>
        /// The unique ID of the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "transaction_id")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Plaid Transaction Type. <see cref="PlaidTransactionType"/>
        /// </summary>
        [JsonProperty(PropertyName = "transaction_type")]
        public PlaidTransactionType TransactionType { get; set; }

        /// <summary>
        /// The ID of the account in which this transaction occurred.
        /// </summary>
        [JsonProperty(PropertyName = "account_id")]
        public string AccountId { get; set; }

        /// <summary>
        /// The settled dollar value. Positive values when money moves out of the account; 
        /// negative values when money moves in. For example, purchases are positive; 
        /// credit card payments, direct deposits, refunds are negative.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// For pending transactions, Plaid returns the date the transaction occurred; 
        /// for posted transactions, Plaid returns the date the transaction posts. 
        /// Both dates are returned in an ISO 8601 format (YYYY-MM-DD).
        /// </summary>
        [JsonProperty(PropertyName = "date")]
        public Date Date { get; set; }

        /// <summary>
        /// A hierarchical array of the categories to which this transaction belongs. 
        /// </summary>
        [JsonProperty(PropertyName = "category")]
        public IList<string> Categories { get; set; }

        /// <summary>
        /// The ID of the category to which this transaction belongs.
        /// </summary>
        [JsonProperty(PropertyName = "category_id")]
        public string CategoryId { get; set; }

        /// <summary>
        /// When true, identifies the transaction as pending or unsettled. 
        /// Pending transaction details (name, type, amount, category ID) may change before they are settled.
        /// </summary>
        [JsonProperty(PropertyName = "pending")]
        public bool IsPending { get; set; }

        /// <summary>
        /// The ID of a posted transaction's associated pending transaction—where applicable.
        /// </summary>
        [JsonProperty(PropertyName = "pending_transaction_id")]
        public string PendingTransactionId { get; set; }

        /// <summary>
        /// The ISO currency code of the transaction. 
        /// Always null if unofficial_currency_code is non-null.
        /// </summary>
        [JsonProperty(PropertyName = "iso_currency_code")]
        public IsoCurrencyCode IsoCurrencyCode { get; set; }

        /// <summary>
        /// The unofficial currency code associated with the transaction. 
        /// Always null if iso_currency_code is non-null.
        /// </summary>
        [JsonProperty(PropertyName = "unofficial_currency_code")]
        public string UnofficialCurrencyCode { get; set; }

        /// <summary>
        /// Information about where the transaction occurred. 
        /// The location key will always be an Object, but no location data elements are guaranteed.
        /// <see cref="PlaidLocationContractV1"/>
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public PlaidLocationContractV1 Location { get; set; }

        /// <summary>
        /// The merchant name or transaction description.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Information about where the transaction occurred. 
        /// The payment_meta key will always be an Object, but no payment_meta data elements are guaranteed.
        /// </summary>
        [JsonProperty(PropertyName = "payment_meta")]
        public PlaidPaymentMetaContractV1 PaymentMeta { get; set; }

        /// <summary>
        /// The name of the account owner. This field is not typically populated and only relevant when dealing with sub-accounts.
        /// </summary>
        [JsonProperty(PropertyName = "account_owner")]
        public string AccountOwner { get; set; }
    }
}
