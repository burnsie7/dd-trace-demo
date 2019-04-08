using System.Collections.Generic;

namespace Datadog.Coffeehouse.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
            => source == null || source.Count == 0;
    }
}
