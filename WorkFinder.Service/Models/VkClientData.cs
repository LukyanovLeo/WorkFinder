using Newtonsoft.Json;
using System.Collections.Generic;

namespace VkService.Models
{
    public class VkClientData
    {
        public List<ClientData> Response;
    }

    public class ClientData
    {
        [JsonProperty("first_name_nom")]
        public string FirstName;

        [JsonProperty("last_name_nom")]
        public string LastName;

        [JsonProperty("city")]
        public City City;

        [JsonProperty("photo_200_orig")]
        public string Photo;
    }

    public class City
    {
        [JsonProperty("title")]
        public string Name { get; set; }
    }
}
