using Microsoft.ML.Data;

namespace TextsClassifierAPI.Models.NeuralNetwork
{
    public class ClassifierItem
    {
        [LoadColumn(0)]
        public string Category { get; set; }
        [LoadColumn(1)]
        public string Text { get; set; }
    }
}
