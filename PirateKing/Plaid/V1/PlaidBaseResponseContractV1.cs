using Newtonsoft.Json;

namespace PirateKing.Plaid.V1
{
    public class PlaidBaseResponseContractV1
    {
        [JsonProperty(PropertyName = "request_Id")]
        public string RequestId { get; set; }
    }
}
