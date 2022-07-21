using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextsClassifierAPI.Models;
using TextsClassifierAPI.Models.NeuralNetwork;
using TextsClassifierAPI.Models.Responses;

namespace TextsClassifierAPI.Helpers
{
    public class NeuralNetworkHelper
    {
        private static string AppPath { get; set; }
        private static string TrainDataPath { get; set; }
        private static string TestDataPath { get; set; }
        private static string ModelPath { get; set; }

        private static MLContext MlContext { get; set; }
        private static PredictionEngine<ClassifierItem, Prediction> PredEngine { get; set; }
        private static ITransformer TrainedModel { get; set; }
        private static IDataView TrainingDataView { get; set; }

        public NeuralNetworkHelper(TextsClassifierOptions config,
                                   string trainDataFile = "vk_texts_train.tsv",
                                   string testDataFile = "vk_texts_test.tsv",
                                   string modelPath = "model.zip")
        {
            AppPath = AppDomain.CurrentDomain.BaseDirectory;
            TrainDataPath = Path.Combine(AppPath, config.NetworkPath, "Data", trainDataFile);
            TestDataPath = Path.Combine(AppPath, config.NetworkPath, "Data", testDataFile);
            ModelPath = Path.Combine(AppPath, config.NetworkPath, "MlModels", modelPath);

            MlContext = new MLContext(seed: 0);
        }

        public IEstimator<ITransformer> PrepareData()
        {
            var pipeline = MlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Category", outputColumnName: "Label")
                .Append(MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Text", outputColumnName: "TextFeaturized"))
                .Append(MlContext.Transforms.Concatenate("Features", "TextFeaturized"))
                .AppendCacheCheckpoint(MlContext);
            return pipeline;
        }

        public IEstimator<ITransformer> BuildAndTrainModel(IEstimator<ITransformer> pipeline)
        {
            var trainingPipeline = pipeline.Append(MlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(MlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            TrainingDataView = MlContext.Data.LoadFromTextFile<ClassifierItem>(TrainDataPath, hasHeader: true);
            TrainedModel = trainingPipeline.Fit(TrainingDataView);
            PredEngine = MlContext.Model.CreatePredictionEngine<ClassifierItem, Prediction>(TrainedModel);

            return trainingPipeline;
            // todo: add logger instaed of console
        }

        public void SaveModelAsFile()
        {
            MlContext.Model.Save(TrainedModel, TrainingDataView.Schema, ModelPath);
        }

        public void Evaluate()
        {
            var testDataView = MlContext.Data.LoadFromTextFile<ClassifierItem>(TestDataPath, hasHeader: true);
            var testMetrics = MlContext.MulticlassClassification.Evaluate(TrainedModel.Transform(testDataView));

            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for Multi-class Classification model - Test Data     ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       MicroAccuracy:    {testMetrics.MicroAccuracy:0.###}");
            Console.WriteLine($"*       MacroAccuracy:    {testMetrics.MacroAccuracy:0.###}");
            Console.WriteLine($"*       LogLoss:          {testMetrics.LogLoss:#.###}");
            Console.WriteLine($"*       LogLossReduction: {testMetrics.LogLossReduction:#.###}");
            Console.WriteLine($"*************************************************************************************************************");
        }

        public ClassificationResponse ClassifyText(string text, string expectedCategory = "")
        {
            ITransformer loadedModel = MlContext.Model.Load(ModelPath, out var modelInputSchema);
            ClassifierItem singleIssue = new ClassifierItem()
            {
                Category = expectedCategory,
                Text = text
            };
            PredEngine = MlContext.Model.CreatePredictionEngine<ClassifierItem, Prediction>(loadedModel);
            var prediction = PredEngine.Predict(singleIssue);

            var highestScore = GetScoresWithLabelsSorted(PredEngine.OutputSchema, "Score", prediction.Score).Values.Max();
            return new ClassificationResponse
            {
                Category = prediction.Category,
                RawText = text,
                Score = highestScore
            };

            //Console.WriteLine($"Result: {prediction.Category}\nExpected category: {expectedCategory}"); // todo: add logger instaed of console
        }

        private static Dictionary<string, float> GetScoresWithLabelsSorted(DataViewSchema schema, string name, float[] scores)
        {
            Dictionary<string, float> result = new Dictionary<string, float>();
            var column = schema.GetColumnOrNull(name);
            var slotNames = new VBuffer<ReadOnlyMemory<char>>();
            column.Value.GetSlotNames(ref slotNames);

            var names = new string[slotNames.Length];
            var num = 0;
            foreach (var denseValue in slotNames.DenseValues())
                result.Add(denseValue.ToString(), scores[num++]);

            return result.OrderByDescending(c => c.Value).ToDictionary(i => i.Key, i => i.Value);
        }
    }
}
