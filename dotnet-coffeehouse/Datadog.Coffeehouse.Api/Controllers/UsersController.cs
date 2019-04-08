using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Datadog.Coffeehouse.Api.Filters;
using Datadog.Coffeehouse.Core.DataAccess;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Coffeehouse.Core.Models;
using Datadog.Trace;
using Datadog.Trace.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.Caching;
using ServiceStack.Redis;

namespace Datadog.Coffeehouse.Api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IDbFactory _dbFactory;
        private readonly IRedisClientsManager _redisClientManager;
        private readonly ICacheClient _cacheClient;
        private readonly Random _random = new Random();

        public UsersController(IUserRepository userRepository,
                               IDbFactory dbFactory,
                               IRedisClientsManager redisClientManager)
        {
            _userRepository = userRepository;
            _dbFactory = dbFactory;
            _redisClientManager = redisClientManager;
            _cacheClient = redisClientManager.GetCacheClient();
        }

        /*
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
            => _userRepository.Select();
        */
        private void Sleep(int minDuration, int? maxDuration = null)
        {
            const double multiplier = 1.35;

            int duration = maxDuration == null
                               ? minDuration
                               : _random.Next(minDuration, maxDuration.Value);

            Thread.Sleep((int)(duration * multiplier));
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            Sleep(30);

            var rootSpan = Tracer.Instance.ActiveScope?.Span;
            rootSpan.SetTag("customer_id", "58325");

            using(var getAllScope = Tracer.Instance.StartActive("user_repository.get_all"))
            {
                Sleep(5);

                for (int i = 0; i < 20; i++)
                {
                    using(var redisScope = Tracer.Instance.StartActive("redis.command", serviceName: "redis"))
                    {
                        redisScope.Span.Type = SpanTypes.Redis;
                        redisScope.Span.ResourceName = "GET urn:User:F7968D5389134E72A8A1B8CD559B5C80";
                        redisScope.Span.SetTag(Tags.OutHost, "redis");
                        redisScope.Span.SetTag(Tags.OutPort, "6379");
                        redisScope.Span.SetTag(Tags.RedisRawCommand, "GET urn:User:F7968D5389134E72A8A1B8CD559B5C80");

                        Sleep(1, 5);
                    }

                    Sleep(1, 2);
                }

                using(var fetchScope = Tracer.Instance.StartActive("users.fetch"))
                {
                    Sleep(10);

                    using(var redisScope = Tracer.Instance.StartActive("redis.command", serviceName: "redis"))
                    {
                        redisScope.Span.Type = SpanTypes.Redis;
                        redisScope.Span.ResourceName = "GET urn:User:F7968D5389134E72A8A1B8CD559B5C80";
                        redisScope.Span.SetTag(Tags.OutHost, "redis");
                        redisScope.Span.SetTag(Tags.OutPort, "6379");
                        redisScope.Span.SetTag(Tags.RedisRawCommand, "GET urn:User:F7968D5389134E72A8A1B8CD559B5C80");

                        Sleep(40);
                    }

                    Sleep(20);

                    using(var sqlScope = Tracer.Instance.StartActive("sql.query", serviceName: "sql-server"))
                    {
                        sqlScope.Span.Type = SpanTypes.Sql;
                        sqlScope.Span.ResourceName = "SELECT Id, Name FROM Users WHERE Address = ?";
                        sqlScope.Span.SetTag(Tags.DbType, "sql-server");
                        sqlScope.Span.SetTag(Tags.DbName, "datadog-users");
                        sqlScope.Span.SetTag(Tags.DbUser, "dd");
                        sqlScope.Span.SetTag(Tags.OutHost, "sqlserver");

                        Sleep(60);
                    }

                    Sleep(20);

                    try
                    {
                        FetchUsers(fetchScope);
                    }
                    catch(Exception ex)
                    {
                        fetchScope.Span.SetException(ex);
                        fetchScope.Span.SetTag(Tags.ErrorStack, GetStackTrace());
                    }
                }

                Sleep(40);
            }

            Sleep(70);

            return new List<User>();
        }

        private static void FetchUsers(Scope fetchScope)
        {
            HandleException();
        }

        private static void HandleException()
        {
            throw new InvalidOperationException();
        }

        [HttpGet("{id}")]
        public ActionResult<User> Get(string id)
            => _userRepository.Single(id);

        [HttpPost]
        public void Post([FromBody] User value)
            => _userRepository.Add(value);

        [HttpPut("{id}")]
        [PutEntityIdMatchedValidationFilter]
        public void Put(string id, [FromBody] User value)
            => _userRepository.Update(value);

        [HttpDelete("{id}")]
        public void Delete(string id)
            => _userRepository.Delete(id);

        private void SleepRandom(int minValue, int maxValue)
        {
            var ms = _random.Next(minValue, minValue);
            Thread.Sleep(ms);
        }

        private string GetStackTrace()
        {
            return @"System.InvalidOperationException: Operation is not valid due to the current state of the object.
     at Datadog.Coffeehouse.Api.Controllers.UsersController.ThrowException(Scope fetchScope) in /usr/src/dotnet-coffeehouse/Datadog.Coffeehouse.Api/Controllers/UsersController.cs:line 121
     at Datadog.Coffeehouse.Api.Controllers.UsersController.Get() in /usr/src/dotnet-coffeehouse/Datadog.Coffeehouse.Api/Controllers/UsersController.cs:line 106
     at lambda_method(Closure , Object , Object[] )
     at Microsoft.AspNetCore.Mvc.Internal.ActionMethodExecutor.SyncObjectResultExecutor.Execute(IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)
     at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeActionMethodAsync()
     at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeNextActionFilterAsync()
     at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Rethrow(ActionExecutedContext context)
     at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
     at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeInnerFilterAsync()
     at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeNextResourceFilter()
     at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.Rethrow(ResourceExecutedContext context)
     at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
     at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeFilterPipelineAsync()
     at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeAsync()
     at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)
     at Microsoft.AspNetCore.Routing.EndpointRoutingMiddleware.Invoke(HttpContext httpContext)
     at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpProtocol.ProcessRequests[TContext](IHttpApplication`1 application)";
        }
    }
}
