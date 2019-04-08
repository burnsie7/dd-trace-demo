using System;
using System.Data;

namespace Datadog.Coffeehouse.Core.Extensions
{
    public static class DbExtensions
    {
        public static void TryDispose(this IDbConnection dbConnection)
        {
            Try.Exec(dbConnection.Close);
            Try.Exec(dbConnection.Dispose);
        }

        public static void TryDispose(this IDisposable disposable)
        {
            Try.Exec(disposable.Dispose);
        }
    }
}
