using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Coffeehouse.Core.Models;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.Redis;

namespace Datadog.Coffeehouse.Core.Extensions
{
    public static class CacheExtensions
    {
        private static readonly ILogger _log = LogManager.GetLogger("CacheExtensions");
        private static readonly TimeSpan _maxTimeToCacheFor = TimeSpan.FromSeconds(45);    // For demo purposes, only honor cache for a little while...

        public static ICollection<T> TryGet<T>(this IRedisClientsManager redisClientManager, Func<ICollection<T>> setGetter,
                                               Func<string, T> singleGetter, string id = "_SELECTALL_")
            where T : class, IHasStringId
        {
            var setCacheKey = UrnId.Create<T>(id);

            // Just taking the simple approach for now of assuming the items in the list are valid if we get the list for now, and the expiration
            // of the list will take care of expiring it on the server side, at which point we won't have a list anymore....good enough for a demo app
            using(var client = redisClientManager.GetClient())
            using(var cacheClient = redisClientManager.GetCacheClient())
            {
                var cachedSetMarker = client.Get<CacheItem<string>>(string.Concat(setCacheKey, "_MARKER"));

                var cachedSet = cachedSetMarker?.Item == null || (DateTime.UtcNow - cachedSetMarker.CachedOn) >= _maxTimeToCacheFor
                                    ? null
                                    : client.GetAllItemsFromSet(setCacheKey);

                if (cachedSet.IsNullOrEmpty())
                {   // Nothing cached for the set, go get them and cache them
                    _log.LogDebug($"CacheSetMiss for [{typeof(T).Name}].[{id}]");

                    var getResults = setGetter();

                    if (getResults.IsNullOrEmpty())
                    {
                        return getResults;
                    }

                    var utcNow = DateTime.UtcNow;

                    client.Set(string.Concat(setCacheKey, "_MARKER"),
                               new CacheItem<string>
                               {
                                   CachedOn = utcNow,
                                   Item = setCacheKey
                               },
                               _maxTimeToCacheFor);
                    
                    getResults.Each(r =>
                                    {
                                        client.AddItemToSet(setCacheKey, r.Id);

                                        cacheClient.Set(
                                                        UrnId.Create<T>(r.Id),
                                                        new CacheItem<T>
                                                        {
                                                            CachedOn = utcNow,
                                                            Item = r
                                                        });
                                    });

                    return getResults;
                }
                
                // Have a cached set, take the IDs and get the items to return
                _log.LogDebug($"CacheSetHit for [{typeof(T).Name}].[{id}]");

                var cachedResults = cachedSet.Select(i => cacheClient.TryGet(i, () => singleGetter(i)))
                                             .AsList();

                return cachedResults;
            }
        }

        public static T TryGet<T>(this ICacheClient cacheClient, string id, Func<T> getter)
            where T : class, IHasStringId
        {
            var cacheKey = UrnId.Create<T>(id);
            var cachedItem = cacheClient.Get<CacheItem<T>>(cacheKey);

            if (cachedItem?.Item != null && (DateTime.UtcNow - cachedItem.CachedOn) < _maxTimeToCacheFor)
            {
                _log.LogDebug($"CacheHit for [{typeof(T).Name}].[{id}]");

                return cachedItem.Item;
            }

            _log.LogDebug($"CacheMiss for [{typeof(T).Name}].[{id}]");

            var item = getter();

            if (!(item?.Id).IsNotNullOrEmpty())
            {
                cacheClient.TryRemove<T>(id);
                return default;
            }

            cacheClient.Set(cacheKey,
                            new CacheItem<T>
                            {
                                CachedOn = DateTime.UtcNow,
                                Item = item
                            },
                            _maxTimeToCacheFor);

            return item;
        }

        public static void TrySet<T>(this ICacheClient cacheClient, T item)
            where T : class, IHasStringId
        {
            var cacheKey = UrnId.Create<T>(item.Id);

            cacheClient.Set(cacheKey,
                            new CacheItem<T>
                            {
                                CachedOn = DateTime.UtcNow,
                                Item = item
                            },
                            _maxTimeToCacheFor);
        }

        public static void TryRemove<T>(this ICacheClient cacheClient, T item)
            where T : class, IHasStringId
            => cacheClient.Remove(UrnId.Create<T>(item.Id));

        public static void TryRemove<T>(this ICacheClient cacheClient, string id)
            => cacheClient.Remove(UrnId.Create<T>(id));

    }
}
