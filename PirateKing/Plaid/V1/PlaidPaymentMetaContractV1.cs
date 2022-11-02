using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    public class PlaidPaymentMetaContractV1
    {
        /// <summary>
        /// The transaction reference number supplied by the financial institution.
        /// </summary>
        [JsonProperty(PropertyName = "reference_number")]
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Payer
        /// </summary>
        [JsonProperty(PropertyName = "payer")]
        public string Payer { get; set; }

        /// <summary>
        /// Payment Method
        /// </summary>
        [JsonProperty(PropertyName = "payment_method")]
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Payment Processor
        /// </summary>
        [JsonProperty(PropertyName = "payment_processor")]
        public string PaymentProcessor { get; set; }

        /// <summary>
        /// The ACH PPD ID for the payer.
        /// </summary>
        [JsonProperty(PropertyName = "ppd_id")]
        public string PpdId { get; set; }

        /// <summary>
        /// Payee
        /// </summary>
        [JsonProperty(PropertyName = "payee")]
        public string Payee { get; set; }

        /// <summary>
        /// For transfers, the party that is receiving the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "payee_name")]
        public string PayeeName { get; set; }

        /// <summary>
        /// By Order Of
        /// </summary>
        [JsonProperty(PropertyName = "by_order_of")]
        public string ByOrderOf { get; set; }

        /// <summary>
        /// Reason
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }
    }
}
