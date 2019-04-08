using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Interfaces;

namespace Datadog.Coffeehouse.Core.DataAccess
{
    public abstract class BaseApiRepository : BaseRepository
    {
        protected BaseApiRepository(IDbFactory dbFactory) : base(dbFactory) { }
        
        protected override AppDatabase AppDbType => AppDatabase.ApiSqlServer;
    }
}
