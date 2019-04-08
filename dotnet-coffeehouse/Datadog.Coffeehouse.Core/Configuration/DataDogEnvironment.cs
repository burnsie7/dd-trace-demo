using System;
using Datadog.Coffeehouse.Core.Extensions;
using Microsoft.AspNetCore.Hosting;

namespace Datadog.Coffeehouse.Core.Configuration
{
    public static class DatadogEnvironment
    {
        private static string _currentEnvironment;

        public static string CurrentEnvironment => _currentEnvironment ?? (_currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                                                                                            .DefaultIfNullOrEmpty(EnvironmentName.Development));

    }
}
