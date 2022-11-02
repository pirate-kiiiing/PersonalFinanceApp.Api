using System;
using PirateKing.Core;
using PirateKing.Guards;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// Guid Date Filter
    /// </summary>
    public class GuidDateFilter : BaseFilter
    {
        /// <summary>
        /// Filter between startDate and endDate
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public GuidDateFilter(Date startDate, Date endDate)
        {
            Validate.ValidRange(startDate, endDate);

            AddToQuery($"(SELECT VALUE SUBSTRING(doc.id, 37, 10) FROM doc) >= @startDate");
            AddToQuery($"(SELECT VALUE SUBSTRING(doc.id, 37, 10) FROM doc) <= @endDate");
            AddParameter("@startDate", startDate.ToString());
            AddParameter("@endDate", endDate.ToString());
        }

        /// <summary>
        /// Filter between startDate and endDate for a guid
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public GuidDateFilter(Guid guid, Date startDate, Date endDate)
        {
            Validate.NotEmpty(guid, nameof(guid));
            Validate.ValidRange(startDate, endDate);

            string startId = new GuidDate(guid, startDate).Id;
            string endId = new GuidDate(guid, endDate).Id;

            AddToQuery("doc.id >= @startId");
            AddToQuery("doc.id <= @endId");
            AddParameter("@startId", startId);
            AddParameter("@endId", endId);
        }
    }
}
