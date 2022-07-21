using Newtonsoft.Json;

namespace VkService.Models
{
    public class VkStreamingData
    {
        [JsonProperty("event")]
        public Event Event;

        [JsonProperty("service_message")]
        public ServiceMessage ServiceMessage { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }

    public class Event
    {
        [JsonProperty("event_type")]
        public string EventType { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("event_id")]
        public EventId EventId { get; set; }
    }

    public class EventId
    {
        [JsonProperty("post_owner_id")]
        public int PosOwnerId { get; set; }
    }

    public class ServiceMessage
    {
        [JsonProperty("service_code")]
        public int ServiceCode { get; set; }
    }
}
