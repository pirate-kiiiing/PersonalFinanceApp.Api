using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// Plaid Transaction Type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlaidTransactionType
    {
        /// <summary>
        /// transactions that took place online. 
        /// </summary>
        digital,

        /// <summary>
        /// transactions that were made at a physical location. 
        /// </summary>
        place,

        /// <summary>
        /// transactions that relate to banks, e.g. fees or deposits. 
        /// </summary>
        special,

        /// <summary>
        /// transactions that do not fit into the other three types.
        /// </summary>
        unresolved,
    }
}
