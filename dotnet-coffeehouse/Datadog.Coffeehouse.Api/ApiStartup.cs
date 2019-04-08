using System;
using System.Collections.Generic;
using Datadog.Coffeehouse.Api.Filters;
using Datadog.Coffeehouse.Core.DataAccess;
using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Coffeehouse.Core.Services;
using Datadog.Trace.ClrProfiler;
using Datadog.Trace.OpenTracing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using ServiceStack.Redis;

namespace Datadog.Coffeehouse.Api
{
    public class ApiStartup
    {
        private readonly IConfiguration _configuration;

        public ApiStartup(ILoggerFactory loggerFactory,
                          IConfiguration configuration)
        {
            _configuration = configuration;
            LogManager.LogFactory = loggerFactory;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o => { o.Filters.Add(new ModelAttributeValidationFilter()); })
                    .SetCompatibilityVersion(CompatibilityVersion.Latest);

            // Demo data creation services
            services.AddSingleton<IDemoDataService, DemoDataService>();

            // Open tracing...
            services.AddSingleton(OpenTracingTracerFactory.CreateTracer());

            services.AddSingleton<IConfigurationDecorationService>(EnvironmentVariableConfigurationDecorationService.Instance);

            services.AddSingleton<SqlServerDbProvider>();
            services.AddSingleton<MySqlDbProvider>();

            services.AddSingleton<IDbFactory>(s => new SimpleDbFactory(GetDbProvidersMap(s)));

            services.AddSingleton<IRedisClientsManager>(
                                                        s => new RedisManagerPool(
                                                                                  EnvironmentVariableConfigurationDecorationService.Instance
                                                                                                                                   .Get("REDIS_HOST")
                                                                                                                                   .DefaultIfNullOrEmpty(_configuration.GetSection("DbConnection.Redis")?.Value)
                                                                                                                                   .DefaultIfNullOrEmpty("localhost:6379"),
                                                                                  new RedisPoolConfig
                                                                                  {
                                                                                      MaxPoolSize = 150
                                                                                  }));

            // Repos
            services.AddSingleton<SqlUserRepository>();
            services.AddSingleton<SqlOrderRepository>();
            services.AddSingleton<SqlOrderItemRepository>();
            services.AddSingleton<MySqlProductRepository>();

            // Manual trace the user repo...
            services.AddSingleton<IUserRepository, ManualTracedSqlUserRepository>(s => new ManualTracedSqlUserRepository(new CachedSqlUserRepository(s.GetRequiredService<IRedisClientsManager>(),
                                                                                                                                                     s.GetRequiredService<SqlUserRepository>())));

            services.AddSingleton<IOrderRepository, CachedSqlOrderRepository>(s => new CachedSqlOrderRepository(s.GetRequiredService<IRedisClientsManager>(),
                                                                                                                s.GetRequiredService<SqlOrderRepository>()));

            services.AddSingleton<IOrderItemRepository, CachedSqlOrderItemRepository>(s => new CachedSqlOrderItemRepository(s.GetRequiredService<IRedisClientsManager>(),
                                                                                                                            s.GetRequiredService<SqlOrderItemRepository>()));

            // Open trace the product repo...
            services.AddSingleton<IProductRepository, OpenTracedMySqlProductRepository>(s => new OpenTracedMySqlProductRepository(s.GetRequiredService<ITracer>(),
                                                                                                                                  new CachedMySqlProductRepository(s.GetRequiredService<IRedisClientsManager>(),
                                                                                                                                                                   s.GetRequiredService<MySqlProductRepository>())));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            if (!Instrumentation.ProfilerAttached)
            {
                LogManager.GetLogger("ApiStartup").LogWarning("Datadog Profiler not attached to current process.");
            }
        }

        private Dictionary<string, IDbProvider> GetDbProvidersMap(IServiceProvider serviceProvider)
            => new Dictionary<string, IDbProvider>(StringComparer.OrdinalIgnoreCase)
               {
                   { AppDatabase.ApiSqlServer.ToString(), serviceProvider.GetRequiredService<SqlServerDbProvider>() },
                   { AppDatabase.ProductMySql.ToString(), serviceProvider.GetRequiredService<MySqlDbProvider>() }
               };
    }
}
