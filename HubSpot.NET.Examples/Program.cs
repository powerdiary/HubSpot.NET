using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HubSpot.NET.Examples
{
    public static class Examples
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.Development.json", true, true);

                    configuration.Build();
                    configuration.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging(logger =>
                    {
                        logger.AddConsole();
                        logger.SetMinimumLevel(LogLevel.Debug);
                    });
                });

        // enable args to be presented from CLI for automated test execution
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args);
            var container = host.Build();
            var configuration = container.Services.GetService<IConfiguration>();

            var demoApi = new DemonstrateApi(configuration["HubSpot:PrivateAppKey"]);
            demoApi.CreateAndDeleteEntities();
        }
    }
}