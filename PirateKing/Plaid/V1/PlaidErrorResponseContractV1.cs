using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    public class PlaidErrorResponseContractV1 : PlaidBaseResponseContractV1
    {
        [JsonProperty(PropertyName = "error_type", Required = Required.Always)]
        public string ErrorType { get; set; }

        [JsonProperty(PropertyName = "error_code", Required = Required.Always)]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "error_message", Required = Required.Always)]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "display_message")]
        public string DisplayMessage { get; set; }

        [JsonProperty(PropertyName = "suggested_action")]
        public string SuggestedAction { get; set; }
    }
}
