using System.Data;

namespace Datadog.Coffeehouse.Core.Interfaces
{
    public interface IDbProvider
    {
        string ProviderType { get; }
        IDbConnection CreateConnection(string namedConnection);
    }
}
