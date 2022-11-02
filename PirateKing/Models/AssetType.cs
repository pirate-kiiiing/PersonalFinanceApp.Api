using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PirateKing.Models
{
    /// <summary>
    /// Asset Types
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AssetType
    {
        Cash,

        Investment,

        Retirement,
    }
}
