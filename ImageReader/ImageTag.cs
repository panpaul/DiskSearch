using System;
using System.IO;
using System.Linq;
using Microsoft.ML;

namespace ImageReader
{
    public class ImageTag
    {
        private readonly string _imagesFolder;
        private readonly string _inceptionTensorFlowModel;
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly string _testTagsTsv;
        private readonly string _trainTagsTsv;


        public ImageTag()
        {
            var assetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
            _imagesFolder = Path.Combine(assetsPath, "images");
            _testTagsTsv = Path.Combine(_imagesFolder, "test-tags.tsv");
            _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
            _inceptionTensorFlowModel = Path.Combine(assetsPath, "inception", "tensorflow_inception_graph.pb");

            _mlContext = new MLContext();
            _model = GenerateModel(_mlContext);
        }

        public string ClassifySingleImage(string imagePath)
        {
            var imageData = new ImageData
            {
                ImagePath = imagePath
            };
            var predictor = _mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(_model);
            try
            {
                var prediction = predictor.Predict(imageData);
                return prediction.PredictedLabelValue;
            }
            catch (Exception)
            {
                return "Broken Image";
            }
        }

        private ITransformer GenerateModel(MLContext mlContext)
        {
            IEstimator<ITransformer> pipeline =
                mlContext.Transforms
                    .LoadImages("input", _imagesFolder, nameof(ImageData.ImagePath))
                    // The image transforms transform the images into the model's expected format.
                    .Append(mlContext.Transforms
                        .ResizeImages("input", InceptionSettings.ImageWidth, InceptionSettings.ImageHeight, "input"))
                    .Append(mlContext.Transforms.ExtractPixels("input",
                        interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
                    .Append(mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel)
                        .ScoreTensorFlowModel(new[] {"softmax2_pre_activation"}, new[] {"input"}, true))
                    .Append(mlContext.Transforms.Conversion.MapValueToKey("LabelKey", "Label"))
                    .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy("LabelKey",
                        "softmax2_pre_activation"))
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                    .AppendCacheCheckpoint(mlContext);
            var trainingData = mlContext.Data.LoadFromTextFile<ImageData>(_trainTagsTsv, hasHeader: false);
            var model = pipeline.Fit(trainingData);
            var testData = mlContext.Data.LoadFromTextFile<ImageData>(_testTagsTsv, hasHeader: false);
            var predictions = model.Transform(testData);

            // Create an IEnumerable for the predictions for displaying results
            //var imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, true);
            //DisplayResults(imagePredictionData);
            var metrics =
                mlContext.MulticlassClassification.Evaluate(predictions,
                    "LabelKey",
                    predictedLabelColumnName: "PredictedLabel");
            Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
            Console.WriteLine(
                $"PerClassLogLoss is: {string.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");
            return model;
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