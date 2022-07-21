using Microsoft.ML.Data;

namespace TextsClassifierAPI.Models.NeuralNetwork
{
    public class Prediction
    {
        [ColumnName("PredictedLabel")]
        public string Category;

        [ColumnName("Score")]
        public float[] Score;
    }

    public class Filtration
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}
