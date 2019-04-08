using System;
using System.Collections.Generic;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using OpenTracing;
using ServiceStack;

namespace Datadog.Coffeehouse.Core.DataAccess
{
    public class OpenTracedApiRepository<T> : IApiRepository<T>
        where T : class, IHasStringId
    {
        private readonly ITracer _tracer;
        private readonly IApiRepository<T> _innerRepository;
        private readonly string _repoName;

        public OpenTracedApiRepository(ITracer tracer, IApiRepository<T> innerRepository)
        {
            _tracer = tracer;
            _innerRepository = innerRepository;
            _repoName = _innerRepository.GetType().Name;
        }

        public T Single(string id)
        {
            using(var scope = _tracer.BuildSpan(string.Concat(_repoName, ".", "Single(", id, ")")).StartActive(true))
            {
                var item = _innerRepository.Single(id);

                scope.Span.SetTag("trace-type", "OpenTracing");
                scope.Span.Log(DateTimeOffset.UtcNow, (item?.ToJsv()).DefaultIfNullOrEmpty("<NULL>"));

                return item;
            }
        }

        public List<T> Select(int limit = 0)
        {
            using(var scope = _tracer.BuildSpan(string.Concat(_repoName, ".", "Select(", limit, ")")).StartActive(true))
            {
                var results = _innerRepository.Select(limit);

                scope.Span.SetTag("trace-type", "OpenTrace");
                scope.Span.Log(DateTimeOffset.UtcNow, string.Concat("ResultCount: [", results?.Count ?? 0, "]"));

                return results;
            }
        }

        public string Add(T item)
        {
            using(var scope = _tracer.BuildSpan(string.Concat(_repoName, ".", "Add(id:", item.Id.DefaultIfNullOrEmpty("<NULL>"), ")")).StartActive(true))
            {
                var id = _innerRepository.Add(item);

                scope.Span.SetTag("trace-type", "OpenTrace");
                scope.Span.Log(DateTimeOffset.UtcNow, (item?.ToJsv()).DefaultIfNullOrEmpty("<NULL>"));

                return id;
            }
        }

        public void Update(T item)
        {
            using(var scope = _tracer.BuildSpan(string.Concat(_repoName, ".", "Update(id:", item.Id.DefaultIfNullOrEmpty("<NULL>"), ")")).StartActive(true))
            {
                _innerRepository.Update(item);

                scope.Span.SetTag("trace-type", "OpenTrace");
                scope.Span.Log(DateTimeOffset.UtcNow, (item?.ToJsv()).DefaultIfNullOrEmpty("<NULL>"));
            }
        }

        public void Delete(string id)
        {
            using(var scope = _tracer.BuildSpan(string.Concat(_repoName, ".", "Delete(", id.DefaultIfNullOrEmpty("<NULL>"), ")")).StartActive(true))
            {
                scope.Span.SetTag("trace-type", "OpenTrace");
                _innerRepository.Delete(id);
            }
        }

        public void InitSchema()
        {
            using(var scope = _tracer.BuildSpan(string.Concat(_repoName, ".", "InitSchema()")).StartActive(true))
            {
                scope.Span.SetTag("trace-type", "OpenTrace");
                _innerRepository.InitSchema();
            }
        }
    }
}
