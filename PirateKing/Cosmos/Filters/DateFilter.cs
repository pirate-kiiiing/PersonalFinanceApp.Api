using PirateKing.Core;
using PirateKing.Guards;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// Date Filter
    /// </summary>
    public class DateFilter : BaseFilter
    {
        /// <summary>
        /// Filter between startDate and endDate
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public DateFilter(Date startDate, Date endDate)
        {
            Validate.ValidRange(startDate, endDate);

            AddToQuery($"doc.date >= @{nameof(startDate)}");
            AddToQuery($"doc.date <= @{nameof(endDate)}");
            AddParameter($"@{nameof(startDate)}", startDate.ToString());
            AddParameter($"@{nameof(endDate)}", endDate.ToString());
        }
    }
}
