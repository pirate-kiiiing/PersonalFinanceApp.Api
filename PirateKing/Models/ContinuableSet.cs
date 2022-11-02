using System.Collections.Generic;

namespace PirateKing.Models
{
    /// <summary>
    /// ContinuableSet class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ContinuableSet<T>
    {
        /// <summary>
        /// Items
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Continuation Token
        /// </summary>
        public string ContinuationToken { get; set; }
    }
}
