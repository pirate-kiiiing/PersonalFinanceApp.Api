using System.Collections.Generic;
using System.Linq;

namespace PirateKing.Core
{
    public static class IEnumerableExtension
    {
        public static bool IsNullOrEmpty<T>(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return (value == null || value.Any() == false);
        }
    }
}
