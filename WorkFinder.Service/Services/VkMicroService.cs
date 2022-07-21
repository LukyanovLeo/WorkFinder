using Microsoft.Extensions.Configuration;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System.Linq;
using System.Reflection;
using VkService.Helpers;
using VkService.Models;

namespace VkService.Services
{
    public class VkMicroService : IMicroService
    {
        private IMicroServiceController controller { get; set; }
        private VkStreamingHelper vkStreamingHelper { get; set; }
        private VkClientHelper vkClientHelper { get; set; }
        private ClassifierHelper classifierHelper { get; set; }

        public VkMicroService()
        {
            controller = null;
        }

        public VkMicroService(IMicroServiceController serviceController)
        {
            controller = serviceController;
            classifierHelper = new ClassifierHelper();
            vkStreamingHelper = new VkStreamingHelper();
            vkClientHelper = new VkClientHelper();
        }

        public void Start()
        {
            LoggerHelper.Logger.Information($"{MethodBase.GetCurrentMethod()}");

            vkStreamingHelper.VkStreamingNotify += GetVkData;
            vkStreamingHelper.GetData();
        }

        public void Stop()
        {
            vkStreamingHelper.StopGetData();
        }

        private void GetVkData(VkStreamingData data)
        {
            LoggerHelper.Logger.Information($"{MethodBase.GetCurrentMethod()}");

            if (data != null && data.Event.EventType == "post" && data.Event.Action == "new" && data.Event.EventId.PosOwnerId > 0)
            {
                var userInfo = vkClientHelper.GetUserInfo(data.Event.EventId.PosOwnerId);
                LoggerHelper.Logger.Information($"Пользователь {userInfo.Response.First().FirstName} {userInfo.Response.First().LastName} c id = {data.Event.EventId.PosOwnerId} добавил новый пост");

                var classification = classifierHelper.GetCategory(data.Event.Text);
            }
        }
    }
}
