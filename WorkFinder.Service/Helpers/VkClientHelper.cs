using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Reflection;
using VkService.Models;

namespace VkService.Helpers
{
    public class VkClientHelper
    {
        public VkClientData GetUserInfo(int userId)
        {
            LoggerHelper.Logger.Information($"{MethodBase.GetCurrentMethod()}");

            var fields = new string[] { "first_name_nom", "last_name_nom", "city", "photo_200_orig" };
            var url = String.Format(ConfigHelper.configuration.GetValue<string>("ClientApi:Url"), userId, String.Join(",", fields), ConfigHelper.configuration.GetValue<string>("ServiceKey"));

            using (var wc = new WebClient())
                return JsonConvert.DeserializeObject<VkClientData>(wc.DownloadString(url));
        }
    }
}
