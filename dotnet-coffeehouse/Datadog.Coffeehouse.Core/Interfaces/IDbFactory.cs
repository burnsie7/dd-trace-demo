using System.Data;

namespace Datadog.Coffeehouse.Core.Interfaces
{
    public interface IDbFactory
    {
        string ProviderType(string forNamedConnection);
        IDbConnection OpenConnection(string namedConnection);
    }
}
