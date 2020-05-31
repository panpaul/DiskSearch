using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;

namespace ImageReader
{
    public class Class1
    {
        private static readonly string AssetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
        private static readonly string ImagesFolder = Path.Combine(AssetsPath, "images");
        private static readonly string TrainTagsTsv = Path.Combine(ImagesFolder, "tags.tsv");
        private static readonly string TestTagsTsv = Path.Combine(ImagesFolder, "test-tags.tsv");
        private static readonly string PredictSingleImage = Path.Combine(ImagesFolder, "toaster3.jpg");

        private static readonly string InceptionTensorFlowModel =
            Path.Combine(AssetsPath, "inception", "tensorflow_inception_graph.pb");


        private void test()
        {
            var mlContext = new MLContext();
            var model = GenerateModel(mlContext);
            ClassifySingleImage(mlContext, model);
        }

        public static ITransformer GenerateModel(MLContext mlContext)
        {
            IEstimator<ITransformer> pipeline = mlContext.Transforms.LoadImages("input",
                    ImagesFolder, nameof(ImageData.ImagePath))
                // The image transforms transform the images into the model's expected format.
                .Append(mlContext.Transforms.ResizeImages("input",
                    InceptionSettings.ImageWidth, InceptionSettings.ImageHeight,
                    "input"))
                .Append(mlContext.Transforms.ExtractPixels("input",
                    interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
                .Append(mlContext.Model.LoadTensorFlowModel(InceptionTensorFlowModel)
                    .ScoreTensorFlowModel(new[] {"softmax2_pre_activation"}, new[] {"input"}, true))
                .Append(mlContext.Transforms.Conversion.MapValueToKey("LabelKey", "Label"))
                .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy("LabelKey",
                    "softmax2_pre_activation"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                .AppendCacheCheckpoint(mlContext);
            var trainingData = mlContext.Data.LoadFromTextFile<ImageData>(TrainTagsTsv, hasHeader: false);
            var model = pipeline.Fit(trainingData);
            var testData = mlContext.Data.LoadFromTextFile<ImageData>(TestTagsTsv, hasHeader: false);
            var predictions = model.Transform(testData);

            // Create an IEnumerable for the predictions for displaying results
            var imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, true);
            DisplayResults(imagePredictionData);
            var metrics =
                mlContext.MulticlassClassification.Evaluate(predictions,
                    "LabelKey",
                    predictedLabelColumnName: "PredictedLabel");
            Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
            Console.WriteLine(
                $"PerClassLogLoss is: {string.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");
            return model;
        }

        private static void DisplayResults(IEnumerable<ImagePrediction> imagePredictionData)
        {
            foreach (var prediction in imagePredictionData)
                Console.WriteLine(
                    $"Image: {Path.GetFileName(prediction.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
        }

        public static IEnumerable<ImageData> ReadFromTsv(string file, string folder)
        {
            return File.ReadAllLines(file)
                .Select(line => line.Split('\t'))
                .Select(line => new ImageData
                {
                    ImagePath = Path.Combine(folder, line[0])
                });
        }

        public static void ClassifySingleImage(MLContext mlContext, ITransformer model)
        {
            var imageData = new ImageData
            {
                ImagePath = PredictSingleImage
            };
            // Make prediction function (input = ImageData, output = ImagePrediction)
            var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
            var prediction = predictor.Predict(imageData);
            Console.WriteLine(
                $"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
        }

        private struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }
    }
}