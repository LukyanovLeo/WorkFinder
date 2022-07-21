using Newtonsoft.Json;
namespace TextsClassifierAPI.Models.NeuralNetwork
{
    public class Text
    {
        [JsonProperty("text")]
        public string Comment { get; set; }
    }
}
