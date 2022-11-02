using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PirateKing.Models
{
    /// <summary>
    /// ISO Currency Codes
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IsoCurrencyCode
    {
        None,

        EUR,

        JPY,

        KRW,

        MXN,

        USD,
    }
}
