using System.Data;
using System.Data.SqlClient;
using Datadog.Coffeehouse.Core.Configuration;
using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Datadog.Coffeehouse.Core.Services
{
    public class SqlServerDbProvider : BaseRdbmsDbProvider
    {
        public SqlServerDbProvider(IOptionsSnapshot<DbConnectionConfig> configFactory,
                                   IConfigurationDecorationService configDecorationService)
            : base(configFactory, configDecorationService) { }
        
        public override string ProviderType => DbProviderType.SqlServer;

        protected override string GetConnectionString(DbConnectionConfig forConfig)
            => $"Server={forConfig.Server};Database={forConfig.Database};User Id={forConfig.UserId};Password={forConfig.Password};Connect Timeout={forConfig.ConnectTimeout.GreaterThanZero(11)};Connection Lifetime={forConfig.ConnectionLifetime.GreaterThanZero(7200)}";

        protected override IDbConnection CreateTypedConnection(string connectionString)
            => new SqlConnection(connectionString);
    }
}
