using Microsoft.ML;
using Microsoft.ML.Data;
using ProjetoChallengeMottu.DTOs;

namespace ProjetoChallengeMottu.Services
{
    /// <summary>
    /// Simple ML.NET regression model that predicts delivery time (in minutes)
    /// based on distance (km), traffic level and number of stops.
    /// Trains once at startup on a small synthetic dataset and serves predictions via DI.
    /// </summary>
    public class DeliveryTimeModel
    {
        private readonly MLContext _ml;
        private readonly ITransformer _model;

        public DeliveryTimeModel()
        {
            _ml = new MLContext(seed: 42);

            // Synthetic training data: DistanceKm, TrafficLevel(0=low,1=medium,2=high), Stops, TempoMinutos (label)
            var samples = new List<ModelInput>
            {
                new() { DistanceKm = 1.0f, TrafficLevel = 0f, Stops = 0f, TempoMinutos = 4f },
                new() { DistanceKm = 2.0f, TrafficLevel = 0f, Stops = 0f, TempoMinutos = 7f },
                new() { DistanceKm = 3.0f, TrafficLevel = 1f, Stops = 1f, TempoMinutos = 14f },
                new() { DistanceKm = 5.0f, TrafficLevel = 1f, Stops = 1f, TempoMinutos = 22f },
                new() { DistanceKm = 8.0f, TrafficLevel = 2f, Stops = 2f, TempoMinutos = 40f },
                new() { DistanceKm = 10.0f, TrafficLevel = 2f, Stops = 3f, TempoMinutos = 55f },
                new() { DistanceKm = 4.0f, TrafficLevel = 0f, Stops = 0f, TempoMinutos = 12f },
                new() { DistanceKm = 6.0f, TrafficLevel = 1f, Stops = 0f, TempoMinutos = 20f },
                new() { DistanceKm = 12.0f, TrafficLevel = 2f, Stops = 1f, TempoMinutos = 58f },
                new() { DistanceKm = 7.0f, TrafficLevel = 1f, Stops = 2f, TempoMinutos = 30f },
            };

            var data = _ml.Data.LoadFromEnumerable(samples);

            var pipeline = _ml.Transforms.Concatenate("Features", nameof(ModelInput.DistanceKm), nameof(ModelInput.TrafficLevel), nameof(ModelInput.Stops))
                .Append(_ml.Regression.Trainers.Sdca(labelColumnName: nameof(ModelInput.TempoMinutos), featureColumnName: "Features"));

            _model = pipeline.Fit(data);
        }

        /// <summary>
        /// Predict delivery time in minutes for the given input.
        /// </summary>
        public DeliveryTimePrediction Predict(DeliveryTimeInput input)
        {
            var engine = _ml.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_model);

            var internalInput = new ModelInput
            {
                DistanceKm = input.DistanceKm,
                TrafficLevel = input.TrafficLevel,
                Stops = input.Stops,
                TempoMinutos = 0f
            };

            var output = engine.Predict(internalInput);
            return new DeliveryTimePrediction { PredictedMinutes = MathF.Max(0f, output.Score) };
        }

        private class ModelInput
        {
            [LoadColumn(0)] public float DistanceKm { get; set; }
            [LoadColumn(1)] public float TrafficLevel { get; set; }
            [LoadColumn(2)] public float Stops { get; set; }
            // Label
            [LoadColumn(3), ColumnName("Label")] public float TempoMinutos { get; set; }
        }

        private class ModelOutput
        {
            [ColumnName("Score")] public float Score { get; set; }
        }
    }
}
