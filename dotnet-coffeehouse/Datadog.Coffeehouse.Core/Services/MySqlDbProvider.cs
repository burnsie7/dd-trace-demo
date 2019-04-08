using System.Data;
using Datadog.Coffeehouse.Core.Configuration;
using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace Datadog.Coffeehouse.Core.Services
{
    public class MySqlDbProvider : BaseRdbmsDbProvider
    {
        public MySqlDbProvider(IOptionsSnapshot<DbConnectionConfig> configFactory,
                               IConfigurationDecorationService configDecorationService)
            : base(configFactory, configDecorationService) { }

        public override string ProviderType => DbProviderType.MySql;

        protected override string GetConnectionString(DbConnectionConfig forConfig)
            => $"Server={forConfig.Server};Port={forConfig.Port.GreaterThanZero(3306)};Database={forConfig.Database};Uid={forConfig.UserId};Pwd={forConfig.Password};Connection Timeout={forConfig.ConnectTimeout.GreaterThanZero(11)};ConnectionLifeTime={forConfig.ConnectionLifetime.GreaterThanZero(7200)};SslMode=Preferred;";

        protected override IDbConnection CreateTypedConnection(string connectionString)
            => new MySqlConnection(connectionString);
    }
}
