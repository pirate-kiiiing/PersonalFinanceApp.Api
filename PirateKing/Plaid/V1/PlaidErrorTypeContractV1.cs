using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PirateKing.Plaid.V1
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlaidErrorTypeContractV1
    {
        NONE,

        INSTITUTION_ERROR,
    }
}
