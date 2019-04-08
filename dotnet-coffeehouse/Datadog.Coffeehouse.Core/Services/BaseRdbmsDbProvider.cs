using System.Collections.Concurrent;
using System.Data;
using Datadog.Coffeehouse.Core.Configuration;
using Datadog.Coffeehouse.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Datadog.Coffeehouse.Core.Services
{
    public abstract class BaseRdbmsDbProvider : IDbProvider
    {
        private readonly IOptionsSnapshot<DbConnectionConfig> _configFactory;
        private readonly IConfigurationDecorationService _configurationDecorationService;
        private readonly ConcurrentDictionary<string, string> _connectionStringMap = new ConcurrentDictionary<string, string>();

        protected BaseRdbmsDbProvider(IOptionsSnapshot<DbConnectionConfig> configFactory,
                                      IConfigurationDecorationService configurationDecorationService)
        {
            _configFactory = configFactory;
            _configurationDecorationService = configurationDecorationService;
        }

        public abstract string ProviderType { get; }
        protected abstract string GetConnectionString(DbConnectionConfig forConfig);
        protected abstract IDbConnection CreateTypedConnection(string connectionString);

        public IDbConnection CreateConnection(string namedConnection)
            => CreateTypedConnection(_connectionStringMap.GetOrAdd(namedConnection,
                                                                   k =>
                                                                   {
                                                                       var config = _configFactory.Get(namedConnection);

                                                                       _configurationDecorationService.Decorate(config, ProviderType);

                                                                       var connectionString = GetConnectionString(config);

                                                                       return connectionString;
                                                                   }));
    }
}
