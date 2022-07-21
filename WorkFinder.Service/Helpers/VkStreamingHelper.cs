using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Reflection;
using System.Text;
using VkService.Models;
using Websocket.Client;

namespace VkService.Helpers
{
    public class VkStreamingHelper
    {
        private WebsocketClient Ws { get; set; }

        public delegate void VkDataHandler(VkStreamingData message);

        public event VkDataHandler VkStreamingNotify;

        private Uri GetStreamingUri()
        {
            using (var wc = new WebClient())
            {
                var streamingConfUrl = String.Format(ConfigHelper.configuration.GetValue<string>("StreamingApi:StreamConfUrl"), ConfigHelper.configuration.GetValue<string>("ServiceKey"));
                var bytes = wc.DownloadData(streamingConfUrl);
                var resp = JsonConvert.DeserializeObject<VKApiAuth>(Encoding.UTF8.GetString(bytes)).Response;
                var streamUrl = String.Format(ConfigHelper.configuration.GetValue<string>("StreamingApi:Url"), resp.Endpoint, resp.Key);

                return new Uri(streamUrl);
            }
        }

        public void GetData()
        {
            Ws = new WebsocketClient(GetStreamingUri());

            Ws.IsReconnectionEnabled = true;
            Ws.ReconnectTimeoutMs = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;
            Ws.ErrorReconnectTimeoutMs = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

            Ws.ReconnectionHappened.Subscribe(type => WebSocketReconnection(type));
            Ws.MessageReceived.Subscribe(msg => WebSocketMessage(msg));
            Ws.DisconnectionHappened.Subscribe(type => WebSocketDisconnection(type));

            Ws.Start();
        }

        public void StopGetData()
        {
            Ws.Dispose();
        }

        private void WebSocketReconnection(ReconnectionType type)
        {
            LoggerHelper.Logger.Information($"{MethodBase.GetCurrentMethod()}");
            LoggerHelper.Logger.Information($"Произошло переподключение, тип: {type}, url: {Ws.Url}");
        }

        private void WebSocketMessage(ResponseMessage msg)
        {
            LoggerHelper.Logger.Information($"{MethodBase.GetCurrentMethod()}");

            var data = JsonConvert.DeserializeObject<VkStreamingData>(msg.ToString());

            LoggerHelper.Logger.Information($"Код ответа:{data.Code}");

            if (data.Code == 300)
            {
                LoggerHelper.Logger.Information($"Ошибка в соединении, код: {data?.ServiceMessage?.ServiceCode}");
                SwitchUrl();
            }
            else
            {
                VkStreamingNotify?.Invoke(data);
            }
        }

        private void WebSocketDisconnection(DisconnectionType type)
        {
            LoggerHelper.Logger.Error($"{MethodBase.GetCurrentMethod()}");
            LoggerHelper.Logger.Error($"Соединение разорванно: {type}");
        }

        private void SwitchUrl()
        {
            LoggerHelper.Logger.Information($"{MethodBase.GetCurrentMethod()}");
            Ws.Url = GetStreamingUri();
            Ws.Reconnect();
        }
    }
}
