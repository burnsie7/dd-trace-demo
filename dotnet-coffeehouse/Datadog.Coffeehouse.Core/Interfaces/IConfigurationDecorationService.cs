using Datadog.Coffeehouse.Core.Configuration;
using System;

namespace Datadog.Coffeehouse.Core.Interfaces
{
    public interface IConfigurationDecorationService
    {
        string Replace(string source, string withEnvironmentVariable, StringComparison comparer = StringComparison.OrdinalIgnoreCase);
        string Get(string environmentVariable);
        void Decorate(DbConnectionConfig dbConfig, string dbProviderType);
    }
}
