using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// Account Tracking Type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlaidWebhookType
    {
        AUTH,

        TRANSACTIONS,

        ITEM,

        INCOME,

        ASSETS,
    }
}
