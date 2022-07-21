using PeterKottas.DotNetCore.WindowsService;
using System.Reflection;
using VkService.Helpers;
using VkService.Services;

namespace VkService
{
    class Program
    {
        public static void Main(string[] args)
        {
            ServiceRunner<VkMicroService>.Run(config =>
            {
                var name = config.GetDefaultName();

                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory((extraArguments, controller) =>
                    {
                        return new VkMicroService(controller);
                    });

                    serviceConfig.OnStart((service, extraParams) =>
                    {
                        LoggerHelper.Logger.Information("Сервис запущен");
                        service.Start();
                    });

                    serviceConfig.OnStop(service =>
                    {
                        service.Stop();
                        LoggerHelper.Logger.Information("Сервис остановлен");
                    });

                    serviceConfig.OnError(e =>
                    {
                        LoggerHelper.Logger.Error($"{MethodBase.GetCurrentMethod()}-Error: {e.Message}");
                    });
                });
            });
        }
    }
}
