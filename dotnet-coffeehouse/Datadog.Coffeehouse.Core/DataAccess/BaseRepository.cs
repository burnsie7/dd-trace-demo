using System;
using System.Data;
using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Trace;

namespace Datadog.Coffeehouse.Core.DataAccess
{
    public abstract class BaseRepository
    {
        protected readonly IDbFactory _dbFactory;

        protected BaseRepository(IDbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        protected virtual string SpanType { get; } = SpanTypes.Sql;
        protected abstract AppDatabase AppDbType { get; }

        protected void DecorateActiveScope()
        {
            var activeScope = Tracer.Instance.ActiveScope;

            if (activeScope == null)
            {
                return;
            }

            activeScope.Span.Type = SpanType;
            activeScope.Span.SetTag(Tags.DbType, _dbFactory.ProviderType(AppDbType.ToString().ToLowerInvariant()));
        }

        protected IDbConnection OpenDbConnection()
            => _dbFactory.OpenConnection(AppDbType.ToString());

        protected T DbExec<T>(Func<IDbConnection, T> sqlExec)
        {
            DecorateActiveScope();

            using(var dbConnection = OpenDbConnection())
            {
                return sqlExec(dbConnection);
            }
        }
    }
}
