using System;
using System.Linq;

namespace Datadog.Coffeehouse.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime _utcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        private static DateTime GetUnspecifiedEpoch(DateTimeKind kind)
            => kind == DateTimeKind.Utc
                   ? _utcEpoch
                   : new DateTime(1970, 1, 1, 0, 0, 0, 0, kind);

        public static long ToUnixTimestamp(this DateTime dateTime, params DateTime?[] coalesce)
        {
            return dateTime >= _utcEpoch
                       ? ToUnixTimestamp(dateTime, GetUnspecifiedEpoch(dateTime.Kind))
                       : coalesce.FirstOrDefault(v => v.HasValue && v.Value >= _utcEpoch)?.ToUnixTimestamp() ?? 0;
        }

        public static long ToUnixTimestamp(this DateTime dateTime, DateTime epoch) => (long)dateTime.Subtract(epoch).TotalSeconds;

        public static string ToSqlString(this DateTime dateTime)
            => dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
