using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// Plaid Location Contract
    /// </summary>
    public class PlaidLocationContractV1
    {
        /// <summary>
        /// The street address where the transaction occurred.
        /// </summary>
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        /// <summary>
        /// The city where the transaction occurred.
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// The region or state where the transaction occurred.
        /// </summary>
        [JsonProperty(PropertyName = "region")]
        public string Region { get; set; }

        /// <summary>
        /// The postal code where the transaction occurred.
        /// </summary>
        [JsonProperty(PropertyName = "postal_code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Zip
        /// </summary>
        [JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; }

        /// <summary>
        /// The ISO 3166-1 alpha-2 country code where the transaction occurred.
        /// </summary>
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        /// <summary>
        /// State
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        /// <summary>
        /// The latitude where the transaction occurred.
        /// </summary>
        [JsonProperty(PropertyName = "lat")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// The longitude where the transaction occurred.
        /// </summary>
        [JsonProperty(PropertyName = "lon")]
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Store number
        /// </summary>
        [JsonProperty(PropertyName = "store_number")]
        public string StoreNumber { get; set; }
    }
}
