using System;
using System.Collections.Generic;
using Dapper;
using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Trace;
using ServiceStack.Caching;
using ServiceStack.Redis;

namespace Datadog.Coffeehouse.Core.DataAccess
{
    public class CachedRedisApiRepository<T> : IApiRepository<T>, IDisposable
        where T : class, IHasStringId
    {
        private readonly ICacheClient _cacheClient;
        private readonly IRedisClientsManager _redisClientManager;
        private readonly IApiRepository<T> _innerRepository;

        public CachedRedisApiRepository(IRedisClientsManager redisClientManager,
                                        IApiRepository<T> innerRepository)
        {
            _redisClientManager = redisClientManager;
            _cacheClient = _redisClientManager.GetCacheClient();
            _innerRepository = innerRepository;
        }

        public void Dispose()
        {
            _redisClientManager.TryDispose();
            _cacheClient.TryDispose();
        }

        public T Single(string id)
            => _cacheClient.TryGet(id, () => _innerRepository.Single(id));

        public List<T> Select(int limit = 0)
            => _redisClientManager.TryGet(() => _innerRepository.Select(limit), i => _innerRepository.Single(i))
                                  .AsList();

        public string Add(T item)
        {
            var id = _innerRepository.Add(item);

            _cacheClient.TrySet(item);

            return id;
        }

        public void Update(T item)
        {
            _cacheClient.TryRemove(item);
            _innerRepository.Update(item);
        }

        public void Delete(string id)
        {
            _cacheClient.TryRemove<T>(id);
            _innerRepository.Delete(id);
        }

        public void InitSchema()
            => _innerRepository.InitSchema();

        private void DecorateActiveScope()
        {
            var activeScope = Tracer.Instance.ActiveScope;

            if (activeScope == null)
            {
                return;
            }

            // If the inner-repos are used to fullfil the request, these will get set there appropriately (i.e. overwritten)
            activeScope.Span.Type = SpanTypes.Redis;
            activeScope.Span.SetTag(Tags.DbType, DbProviderType.Redis.ToLowerInvariant());
        }

    }
}
