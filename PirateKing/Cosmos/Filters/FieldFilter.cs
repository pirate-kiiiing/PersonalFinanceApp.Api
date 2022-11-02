using PirateKing.Guards;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// Field Filter
    /// </summary>
    public class FieldFilter : BaseFilter
    {
        private int counter = 1;

        /// <summary>
        /// Filter on a specific field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        public FieldFilter(string fieldName, string fieldValue, string operation = "=")
        {
            Add(fieldName, fieldValue, operation);
        }

        /// <summary>
        /// Adds a specific query filter
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="operation"></param>
        public void Add(string fieldName, string fieldValue, string operation = "=")
        {
            Validate.NotNullOrEmpty(fieldName, nameof(fieldName));

            AddToQuery($"doc.{fieldName} {operation} @value{counter}");
            AddParameter($"@value{counter}", fieldValue);

            counter++;
        }
    }
}
