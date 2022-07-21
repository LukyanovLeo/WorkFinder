using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace VkService.Helpers
{
    static class ConfigHelper
    {
        public static IConfigurationRoot configuration { get; set; }

        static ConfigHelper()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables();

            configuration = builder.Build();
        }
    }
}
