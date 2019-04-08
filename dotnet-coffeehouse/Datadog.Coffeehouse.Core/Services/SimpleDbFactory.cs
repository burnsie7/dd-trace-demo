using System;
using System.Collections.Generic;
using System.Data;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;

namespace Datadog.Coffeehouse.Core.Services
{
    public class SimpleDbFactory : IDbFactory
    {
        private readonly Dictionary<string, IDbProvider> _providers;

        public SimpleDbFactory(Dictionary<string, IDbProvider> providers)
        {
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        }

        public string ProviderType(string forNamedConnection)
            => _providers.GetValueOrDefault(forNamedConnection)?.ProviderType;

        public IDbConnection OpenConnection(string namedConnection)
        {
            if (!namedConnection.IsNotNullOrEmpty() || !_providers.ContainsKey(namedConnection))
            {
                throw new ArgumentOutOfRangeException(nameof(namedConnection));
            }

            IDbConnection dbConnection = null;

            try
            {
                dbConnection = _providers[namedConnection].CreateConnection(namedConnection);

                dbConnection.Open();

                return dbConnection;
            }
            catch
            {
                dbConnection.TryDispose();

                throw;
            }
        }
    }
}
