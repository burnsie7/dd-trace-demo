using System;
using Microsoft.Extensions.Logging;

namespace Datadog.Coffeehouse.Core.Extensions
{
    public static class Try
    {
        private static readonly ILogger _log = LogManager.GetLogger("Try");

        public static void Exec(Action block)
        {
            try
            {
                block();
            }
            catch(Exception x)
            {
                _log.LogError(x, "Unable to Try.Exec successfully");
            }
        }
    }
}
