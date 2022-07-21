using Dwh;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Reflection;
using System.Text;
using VkService.Models.Db;
using VkService.Models.Request;
using VkService.Models.Responses;

namespace VkService.Helpers
{
    public class ClassifierHelper
    {
        public ClassificationResponse GetCategory(string textForClassify)
        {
            LoggerHelper.Logger.Information($"{MethodBase.GetCurrentMethod()}");
            
            var data = JsonConvert.SerializeObject(new ClassificationRequest(textForClassify));

            using (var wc = new WebClient())
            {
                wc.Headers.Set("Content-Type", "application/json");

                try
                {
                    var responseBytes = wc.UploadData(ConfigHelper.configuration.GetValue<string>("ClassifierApi"), "POST", Encoding.UTF8.GetBytes(data));
                    var responseBody = JsonConvert.DeserializeObject<ClassificationResponse>(Encoding.UTF8.GetString(responseBytes));

                    var dwhClient = new DwhClient("change_me", "change_me", "change_me", 1, "change_me", DbVendor.Postgres);

                    dwhClient.Insert("ml", "vk_results", new TextClassifierResult
                    {
                        Id = Guid.NewGuid(),
                        AddedDate = DateTime.Now,
                        Category = responseBody.Category,
                        RawText = textForClassify,
                        Score = responseBody.Score,
                        Trivia = null,
                    }, false);

                    LoggerHelper.ClassifierLogger.Information($"\nТекст:\n {textForClassify}\n Определен как: \n 1)Категория - {responseBody.Category}\n 2)Вероятность - {responseBody.Score}");
                    LoggerHelper.Logger.Information("Категория полученна");

                    return responseBody;
                } 
                catch(WebException e)
                {
                    LoggerHelper.Logger.Error("При получении категории произошла ошибка");
                    LoggerHelper.Logger.Error(e.Message);
                    return null;
                }
            }
        }
    }
}
