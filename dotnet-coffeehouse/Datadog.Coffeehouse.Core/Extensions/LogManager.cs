using Microsoft.Extensions.Logging;

namespace Datadog.Coffeehouse.Core.Extensions
{
    public static class LogManager
    {
        public static ILoggerFactory LogFactory { get; set; }
        public static ILogger GetLogger<T>() => LogFactory.CreateLogger<T>();
        public static ILogger GetLogger(string loggerName) => LogFactory.CreateLogger(loggerName);
    }
}
