using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PirateKing.Models
{
    /// <summary>
    /// Expense Types
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExpenseType
    {
        Cash,

        Checking,

        Saving,

        Credit,
    }
}
