using System;

namespace Datadog.Coffeehouse.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNotNullOrEmpty(this string source)
            => !string.IsNullOrEmpty(source);

        public static string DefaultIfNullOrEmpty(this string source, string valueIfNullOrEmpty)
            => source.IsNotNullOrEmpty()
                   ? source
                   : valueIfNullOrEmpty;

        public static bool EqualsOrdinalIgnoreCase(this string first, string second)
            => first.Equals(second, StringComparison.OrdinalIgnoreCase);
    }
}
