using System;
using System.IO;
using System.Threading.Tasks;
using Datadog.Coffeehouse.Core.Configuration;
using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Datadog.Coffeehouse.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            var hostConfiguration = BuildConfiguration(new ConfigurationBuilder(), DatadogEnvironment.CurrentEnvironment).Build();
            var builder = new WebHostBuilder().UseKestrel()
                                              .UseIISIntegration()
                                              .UseContentRoot(Directory.GetCurrentDirectory())
                                              .UseStartup<ApiStartup>()
                                              .UseConfiguration(hostConfiguration)
                                              .UseShutdownTimeout(TimeSpan.FromSeconds(15))
                                              .UseUrls("http://*:8084")
                                              .ConfigureAppConfiguration((wc, conf) => BuildConfiguration(conf, DatadogEnvironment.CurrentEnvironment))
                                              .ConfigureServices((hostContext, services) =>
                                                                 {
                                                                     services.AddLogging();

                                                                     services.Configure<DbConnectionConfig>(AppDatabase.ApiSqlServer.ToString(),
                                                                                                            hostContext.Configuration.GetSection("DbConnection.Api"));

                                                                     services.Configure<DbConnectionConfig>(AppDatabase.ProductMySql.ToString(),
                                                                                                            hostContext.Configuration.GetSection("DbConnection.Product"));

                                                                 })
                                              .UseSerilog()
                                              .UseDefaultServiceProvider((x, o) => o.ValidateScopes = false)
                                              .UseStartup<ApiStartup>();

            var built = builder.Build();

            // Background some seed data
            var demoDataService = built.Services.GetRequiredService<IDemoDataService>();

            Task.Run(() => demoDataService.CreateDemoDataBatchAsync(withUsers: true, maxAttempts: 5));

            built.Run();
        }

        private static IConfigurationBuilder BuildConfiguration(IConfigurationBuilder conf, string envName)
            => conf.AddJsonFile("appsettings.json", false, true)
                   .AddJsonFile($"appsettings.{envName}.json", true, true)
                   .AddEnvironmentVariables()
                   .SetBasePath(Directory.GetCurrentDirectory());
    }
}
