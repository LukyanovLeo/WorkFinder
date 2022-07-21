using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace VkService.Helpers
{
    static class LoggerHelper
    {
        public static Logger Logger;
        public static Logger ClassifierLogger;

        static LoggerHelper()
        {
            var basePath = ConfigHelper.configuration.GetValue<string>("LogPath");

            Logger = new LoggerConfiguration().WriteTo.File($@"{basePath}\Logs\VkService.txt").CreateLogger();
            ClassifierLogger = new LoggerConfiguration().WriteTo.File($@"{basePath}\Logs\Classifier.txt").CreateLogger();
        }
    }
}
