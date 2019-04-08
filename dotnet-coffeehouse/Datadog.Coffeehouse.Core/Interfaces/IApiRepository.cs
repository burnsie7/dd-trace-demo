using System.Collections.Generic;

namespace Datadog.Coffeehouse.Core.Interfaces
{
    public interface IApiRepository<T>
    {
        void InitSchema();
        T Single(string id);
        List<T> Select(int limit = 0);
        string Add(T entity);
        void Update(T user);
        void Delete(string id);
    }
}
