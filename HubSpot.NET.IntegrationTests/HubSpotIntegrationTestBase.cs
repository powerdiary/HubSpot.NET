using Microsoft.Extensions.Configuration;

namespace HubSpot.NET.IntegrationTests;

public class HubSpotIntegrationTestBase
{
        protected string? ApiKey;

        protected HubSpotIntegrationTestBase()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .Build();

            ApiKey = configuration["HubSpot:PrivateAppKey"];
        }
}