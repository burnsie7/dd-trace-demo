using System.Collections.Generic;
using Datadog.Coffeehouse.Core.Interfaces;

namespace Datadog.Coffeehouse.Core.DataAccess
{
    public class NullApiRepository<T> : IApiRepository<T>
        where T : class, IHasStringId
    {
        private readonly IApiRepository<T> _innerRepository;

        public NullApiRepository(IApiRepository<T> innerRepository)
        {
            _innerRepository = innerRepository;
        }

        public T Single(string id)
            => _innerRepository.Single(id);

        public List<T> Select(int limit = 0)
            => _innerRepository.Select(limit);

        public string Add(T item)
            => _innerRepository.Add(item);

        public void Update(T item)
            => _innerRepository.Update(item);

        public void Delete(string id)
            => _innerRepository.Delete(id);

        public void InitSchema()
            => _innerRepository.InitSchema();
    }
}
