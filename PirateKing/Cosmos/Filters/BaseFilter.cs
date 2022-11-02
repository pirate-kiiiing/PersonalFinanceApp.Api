using System;
using System.Collections.Generic;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// Base Filter
    /// </summary>
    public class BaseFilter
    {
        private readonly List<string> query;
        private readonly List<KeyValuePair<string, string>> parameters;

        public IEnumerable<KeyValuePair<string, string>> Parameters => parameters;
        public string Query => String.Join(" AND ", query);

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseFilter()
        {
            query = new List<string>
            {
                $"SELECT * FROM doc WHERE doc.id != null",
            };

            parameters = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Add Parameter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void AddParameter(string key, string value)
        {
            parameters.Add(new KeyValuePair<string, string>(key, value));
        }

        /// <summary>
        /// Add to query
        /// </summary>
        /// <param name="expression"></param>
        protected void AddToQuery(string expression)
        {
            query.Add(expression);
        }
    }
}
