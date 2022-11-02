using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PirateKing.Models
{
    /// <summary>
    /// Account Tracking Type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TrackingType
    {
        Job,

        Plaid,
    }
}
