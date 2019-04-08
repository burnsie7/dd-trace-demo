using System;

namespace Datadog.Coffeehouse.Core.Models
{
    public class CacheItem<T>
    {
        public T Item { get; set; }
        public DateTime CachedOn { get; set; }
    }
}
