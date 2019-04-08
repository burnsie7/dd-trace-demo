using System;
using System.Collections.Generic;
using Datadog.Coffeehouse.Core.Configuration;
using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using ServiceStack;
using StringExtensions = Datadog.Coffeehouse.Core.Extensions.StringExtensions;

namespace Datadog.Coffeehouse.Core.Services
{
    public class EnvironmentVariableConfigurationDecorationService : IConfigurationDecorationService
    {
        private readonly Dictionary<string, string> _environmentVariableMap = new Dictionary<string, string>();

        private const string _dbHostSuffix = "_HOST";
        private const string _dbNameSuffix = "_DB_NAME";
        private const string _dbPortSuffix = "_PORT";
        private const string _dbUserNameSuffix = "_USER_NAME";
        private const string _dbPasswordSuffix = "_PASSWORD";
        private const string _demoDdApiKey = "DEMO_DD_API_KEY";

        private EnvironmentVariableConfigurationDecorationService()
        {
            MapEnvironmentVariable(_demoDdApiKey);
            MapEnvironmentVariable(DbProviderType.Redis + _dbHostSuffix);

            MapStandardDbEnvironmentVariablesForPrefix(DbProviderType.SqlServer);
            MapStandardDbEnvironmentVariablesForPrefix(DbProviderType.MySql);
            MapStandardDbEnvironmentVariablesForPrefix(DbProviderType.Mongo);
            MapStandardDbEnvironmentVariablesForPrefix(DbProviderType.ElasticSearch);
        }

        public static EnvironmentVariableConfigurationDecorationService Instance { get; } = new EnvironmentVariableConfigurationDecorationService();

        public string Get(string environmentVariable)
            => _environmentVariableMap.GetValueOrDefault(environmentVariable);

        public string Replace(string source, string withEnvironmentVariable, StringComparison comparer = StringComparison.OrdinalIgnoreCase)
        {
            var envValue = _environmentVariableMap.GetValueOrDefault(withEnvironmentVariable) ?? string.Empty;

            var newValue = source.Replace(source, envValue, comparer);

            return newValue;
        }

        public void Decorate(DbConnectionConfig dbConfig, string dbProviderType)
        {
            dbConfig.Server = Get(dbProviderType + _dbHostSuffix).DefaultIfNullOrEmpty(dbConfig.Server);
            dbConfig.Database = Get(dbProviderType + _dbNameSuffix).DefaultIfNullOrEmpty(dbConfig.Database);
            dbConfig.UserId = Get(dbProviderType + _dbUserNameSuffix).DefaultIfNullOrEmpty(dbConfig.UserId);
            dbConfig.Port = Get(dbProviderType + _dbPortSuffix).ToInt(-1).GreaterThanZero(dbConfig.Port);

            // If a specific password is set, use it period. Otherwise, take the configured value and append the API KEY var if we have one
            var envPwValue = Get(dbProviderType + _dbPasswordSuffix);

            if (StringExtensions.IsNotNullOrEmpty(envPwValue))
            {
                dbConfig.Password = envPwValue;

                return;
            }

            var apiKey = Get(_demoDdApiKey) ?? string.Empty;

            dbConfig.Password = dbConfig.Password + apiKey;
        }
        
        private void MapStandardDbEnvironmentVariablesForPrefix(string prefix)
        {
            MapEnvironmentVariable(prefix + _dbHostSuffix);
            MapEnvironmentVariable(prefix + _dbNameSuffix);
            MapEnvironmentVariable(prefix + _dbPortSuffix);
            MapEnvironmentVariable(prefix + _dbUserNameSuffix);
            MapEnvironmentVariable(prefix + _dbPasswordSuffix);
        }

        private void MapEnvironmentVariable(string environmentVariableName)
        {
            var envValue = Environment.GetEnvironmentVariable(environmentVariableName);

            if (StringExtensions.IsNotNullOrEmpty(envValue))
            {
                _environmentVariableMap[environmentVariableName] = envValue;
            }
        }

    }
}
