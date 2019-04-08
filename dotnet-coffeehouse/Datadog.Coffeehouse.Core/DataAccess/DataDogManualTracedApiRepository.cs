using System;
using System.Collections.Generic;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Trace;

namespace Datadog.Coffeehouse.Core.DataAccess
{
    public class DatadogManualTracedApiRepository<T> : IApiRepository<T>
        where T : class, IHasStringId
    {
        private readonly IApiRepository<T> _innerRepository;
        private readonly string _repoName;

        public DatadogManualTracedApiRepository(IApiRepository<T> innerRepository)
        {
            _innerRepository = innerRepository;
            _repoName = _innerRepository.GetType().Name;
        }

        private bool TraceErrorMessage(Exception x, Scope scope)
        {
            scope.Span.Error = true;
            scope.Span.SetTag(Tags.ErrorMsg, x.Message);
            scope.Span.SetTag(Tags.ErrorType, x.GetType().Name);
            scope.Span.SetTag(Tags.ErrorStack, x.StackTrace);

            return false;
        }

        private void DecorateScope(Scope scope, string resourceName)
        {
            scope.Span.ResourceName = resourceName.DefaultIfNullOrEmpty("<NULL>");
            scope.Span.SetTag("trace-type", "Manual");
        }

        public T Single(string id)
        {
            using(var scope = Tracer.Instance.StartActive(string.Concat(_repoName, ".", "Single(", id, ")")))
            {
                DecorateScope(scope, id);

                try
                {
                    var item = _innerRepository.Single(id);

                    scope.Span.SetTag(Tags.SqlRows, item == null
                                                        ? "0"
                                                        : "1");

                    return item;
                }
                catch(Exception x) when(TraceErrorMessage(x, scope))
                {
                    throw;
                }
            }
        }

        public List<T> Select(int limit = 0)
        {
            using(var scope = Tracer.Instance.StartActive(string.Concat(_repoName, ".", "Select(", limit, ")")))
            {
                DecorateScope(scope, limit.ToString());

                try
                {
                    var results = _innerRepository.Select(limit);

                    scope.Span.SetTag(Tags.SqlRows, (results?.Count).GreaterThanZero(0).ToString());

                    return results;
                }
                catch(Exception x) when(TraceErrorMessage(x, scope))
                {
                    throw;
                }
            }
        }

        public string Add(T item)
        {
            using(var scope = Tracer.Instance.StartActive(string.Concat(_repoName, ".", "Add(id:", item.Id.DefaultIfNullOrEmpty("<NULL>"), ")")))
            {
                DecorateScope(scope, item?.Id);

                try
                {
                    var id = _innerRepository.Add(item);

                    scope.Span.SetTag(Tags.SqlRows, id.IsNotNullOrEmpty()
                                                        ? "1"
                                                        : "0");

                    return id;
                }
                catch(Exception x) when(TraceErrorMessage(x, scope))
                {
                    throw;
                }
            }
        }

        public void Update(T item)
        {
            using(var scope = Tracer.Instance.StartActive(string.Concat(_repoName, ".", "Update(id:", item.Id.DefaultIfNullOrEmpty("<NULL>"), ")")))
            {
                DecorateScope(scope, item.Id);

                try
                {
                    _innerRepository.Add(item);

                    scope.Span.SetTag(Tags.SqlRows, "1");
                }
                catch(Exception x) when(TraceErrorMessage(x, scope))
                {
                    throw;
                }
            }
        }

        public void Delete(string id)
        {
            using(var scope = Tracer.Instance.StartActive(string.Concat(_repoName, ".", "Delete(", id.DefaultIfNullOrEmpty("<NULL>"), ")")))
            {
                DecorateScope(scope, id);

                try
                {
                    _innerRepository.Delete(id);

                    scope.Span.SetTag(Tags.SqlRows, "1");
                }
                catch(Exception x) when(TraceErrorMessage(x, scope))
                {
                    throw;
                }
            }
        }

        public void InitSchema()
        {
            using(var scope = Tracer.Instance.StartActive(string.Concat(_repoName, ".", "InitSchema()")))
            {
                DecorateScope(scope, null);

                try
                {
                    _innerRepository.InitSchema();

                    scope.Span.SetTag(Tags.SqlRows, "0");
                }
                catch(Exception x) when(TraceErrorMessage(x, scope))
                {
                    throw;
                }
            }
        }
    }
}
