using Microsoft.ML;
using Microsoft.ML.Data;
using ProjetoChallengeMottu.DTOs;

namespace ProjetoChallengeMottu.Services
{
    /// <summary>
    /// Modelo simples de regressão para estimar o tempo (min) para localizar uma moto no pátio.
    /// Features:
    /// - PatioAreaM2: área do pátio em m²
    /// - MotosNoPatio: quantidade de motos presentes
    /// - PercentualComBeacon: fração de motos com EchoBeacon ativo (0..1)
    /// - FuncionariosBuscando: número de funcionários envolvidos na busca
    /// - HoraPico: 1 se é horário de pico, 0 caso contrário
    /// </summary>
    public class PatioSearchTimeModel
    {
        private readonly MLContext _ml;
        private readonly ITransformer _model;

        public PatioSearchTimeModel()
        {
            _ml = new MLContext(seed: 42);

            var samples = new List<ModelInput>
            {
                // PatioArea, Motos, %Beacon, Funcionarios, HoraPico, TempoBuscaMin
                new() { PatioAreaM2 = 800f,  MotosNoPatio = 30f, PercentualComBeacon = 0.8f, FuncionariosBuscando = 2f, HoraPico = 0f, TempoBuscaMin = 3.5f },
                new() { PatioAreaM2 = 1200f, MotosNoPatio = 50f, PercentualComBeacon = 0.7f, FuncionariosBuscando = 2f, HoraPico = 1f, TempoBuscaMin = 7.5f },
                new() { PatioAreaM2 = 2000f, MotosNoPatio = 80f, PercentualComBeacon = 0.9f, FuncionariosBuscando = 3f, HoraPico = 0f, TempoBuscaMin = 6.0f },
                new() { PatioAreaM2 = 2000f, MotosNoPatio = 120f,PercentualComBeacon = 0.5f, FuncionariosBuscando = 2f, HoraPico = 1f, TempoBuscaMin = 12.0f },
                new() { PatioAreaM2 = 1000f, MotosNoPatio = 40f, PercentualComBeacon = 0.6f, FuncionariosBuscando = 1f, HoraPico = 0f, TempoBuscaMin = 6.0f },
                new() { PatioAreaM2 = 1500f, MotosNoPatio = 60f, PercentualComBeacon = 0.9f, FuncionariosBuscando = 2f, HoraPico = 0f, TempoBuscaMin = 4.5f },
                new() { PatioAreaM2 = 3000f, MotosNoPatio = 150f,PercentualComBeacon = 0.6f, FuncionariosBuscando = 3f, HoraPico = 1f, TempoBuscaMin = 14.0f },
                new() { PatioAreaM2 = 500f,  MotosNoPatio = 20f, PercentualComBeacon = 0.4f, FuncionariosBuscando = 1f, HoraPico = 0f, TempoBuscaMin = 5.5f },
                new() { PatioAreaM2 = 2500f, MotosNoPatio = 100f,PercentualComBeacon = 1.0f, FuncionariosBuscando = 4f, HoraPico = 0f, TempoBuscaMin = 5.0f },
                new() { PatioAreaM2 = 2500f, MotosNoPatio = 100f,PercentualComBeacon = 0.3f, FuncionariosBuscando = 2f, HoraPico = 1f, TempoBuscaMin = 13.0f },
            };

            var data = _ml.Data.LoadFromEnumerable(samples);

            var pipeline = _ml.Transforms.Concatenate("Features",
                    nameof(ModelInput.PatioAreaM2),
                    nameof(ModelInput.MotosNoPatio),
                    nameof(ModelInput.PercentualComBeacon),
                    nameof(ModelInput.FuncionariosBuscando),
                    nameof(ModelInput.HoraPico))
                .Append(_ml.Regression.Trainers.Sdca(labelColumnName: nameof(ModelInput.TempoBuscaMin), featureColumnName: "Features"));

            _model = pipeline.Fit(data);
        }

        public PatioSearchPrediction Predict(PatioSearchInput input)
        {
            var engine = _ml.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_model);
            var internalInput = new ModelInput
            {
                PatioAreaM2 = input.PatioAreaM2,
                MotosNoPatio = input.MotosNoPatio,
                PercentualComBeacon = input.PercentualComBeacon,
                FuncionariosBuscando = input.FuncionariosBuscando,
                HoraPico = input.HoraPico,
                TempoBuscaMin = 0f
            };
            var output = engine.Predict(internalInput);
            return new PatioSearchPrediction { PredictedMinutes = MathF.Max(0f, output.Score) };
        }

        private class ModelInput
        {
            [LoadColumn(0)] public float PatioAreaM2 { get; set; }
            [LoadColumn(1)] public float MotosNoPatio { get; set; }
            [LoadColumn(2)] public float PercentualComBeacon { get; set; }
            [LoadColumn(3)] public float FuncionariosBuscando { get; set; }
            [LoadColumn(4)] public float HoraPico { get; set; }
            [LoadColumn(5)] public float TempoBuscaMin { get; set; }
        }

        private class ModelOutput
        {
            [ColumnName("Score")] public float Score { get; set; }
        }
    }
}
