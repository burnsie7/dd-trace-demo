using System;

namespace Datadog.Coffeehouse.Core.Extensions
{
    public static class GuidExtensions
    {
        public static string ToStringId(this Guid source)
            => source.ToString("N").ToUpperInvariant();
    }
}
