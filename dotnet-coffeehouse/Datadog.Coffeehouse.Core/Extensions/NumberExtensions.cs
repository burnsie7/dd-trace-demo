using System;

namespace Datadog.Coffeehouse.Core.Extensions
{
    public static class NumberExtensions
    {
        public static int GreaterThanZero(this int? source, int defaultValue)
            => source.HasValue
                   ? GreaterThanZero(source.Value, defaultValue)
                   : defaultValue;

        public static int GreaterThanZero(this int source, int defaultValue)
            => source > 0
                   ? source
                   : defaultValue;

        public static double GreaterThanZero(this double source, int defaultValue)
            => source > 0
                   ? source
                   : defaultValue;

        public static void Times(this int source, Action block)
            => Times(source, i => block());

        public static void Times(this int source, Action<int> block)
        {
            for (var i = 1; i <= source; i++)
            {
                block(i);
            }
        }
    }
}
