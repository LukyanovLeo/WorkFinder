using Newtonsoft.Json;

namespace VkService.Models
{
    class VKApiAuth
    {
        [JsonProperty]
        public Response Response { get; set; }
    }

    public class Response
    {
        [JsonProperty]
        public string Endpoint { get; set; }

        [JsonProperty]
        public string Key { get; set; }
    }
}
