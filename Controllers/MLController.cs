using Microsoft.AspNetCore.Mvc;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Services;

namespace ProjetoChallengeMottu.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/ml")]
    [ApiVersion("1.0")]
    public class MLController : ControllerBase
    {
        private readonly PatioSearchTimeModel _patioModel;

        public MLController(PatioSearchTimeModel patioModel)
        {
            _patioModel = patioModel;
        }

        

        /// <summary>
        /// Estima o tempo (em minutos) para localizar uma moto no pátio.
        /// </summary>
        [HttpPost("predict-search-time")]
        public ActionResult<PatioSearchPrediction> PredictSearchTime([FromBody] PatioSearchInput input)
        {
            if (input is null) return BadRequest("Dados de entrada são obrigatórios.");
            if (input.PatioAreaM2 < 0 || input.MotosNoPatio < 0 || input.PercentualComBeacon < 0 || input.FuncionariosBuscando < 0)
                return BadRequest("Valores devem ser não negativos.");
            if (input.PercentualComBeacon > 100) return BadRequest("PercentualComBeacon deve estar entre 0 e 1 (fração) ou 0 e 100 (%).");
            if (input.HoraPico != 0 && input.HoraPico != 1) return BadRequest("HoraPico deve ser 0 ou 1.");
            var normalized = new PatioSearchInput
            {
                PatioAreaM2 = input.PatioAreaM2,
                MotosNoPatio = input.MotosNoPatio,
                PercentualComBeacon = input.PercentualComBeacon > 1 ? input.PercentualComBeacon / 100f : input.PercentualComBeacon,
                FuncionariosBuscando = input.FuncionariosBuscando,
                HoraPico = input.HoraPico
            };

            var result = _patioModel.Predict(normalized);
            return Ok(result);
        }

        /// <summary>
        /// Estima a economia mensal (R$) com base no tempo previsto de busca e parâmetros operacionais.
        /// </summary>
        [HttpPost("predict-savings")]
        public ActionResult<SavingsPrediction> PredictSavings([FromBody] SavingsInput input)
        {
            if (input is null) return BadRequest("Dados de entrada são obrigatórios.");
            if (input.BaselineTimePerSearchMin < 0 || input.SearchesPerDay < 0 || input.HourlyCostBRL < 0)
                return BadRequest("Parâmetros de baseline/custo devem ser não negativos.");
            if (input.PercentualComBeacon < 0 || input.PercentualComBeacon > 100)
                return BadRequest("PercentualComBeacon deve estar entre 0 e 1 (fração) ou 0 e 100 (%).");

            var normalized = new PatioSearchInput
            {
                PatioAreaM2 = input.PatioAreaM2,
                MotosNoPatio = input.MotosNoPatio,
                PercentualComBeacon = input.PercentualComBeacon > 1 ? input.PercentualComBeacon / 100f : input.PercentualComBeacon,
                FuncionariosBuscando = input.FuncionariosBuscando,
                HoraPico = input.HoraPico
            };

            var searchTime = _patioModel.Predict(normalized).PredictedMinutes;
            var minutesSaved = MathF.Max(0f, input.BaselineTimePerSearchMin - searchTime);
            var dailySavingsBRL = (minutesSaved / 60f) * input.HourlyCostBRL * input.SearchesPerDay;
            // Aproximação: 22 dias úteis/mês
            var monthlySavingsBRL = dailySavingsBRL * 22f;

            return Ok(new SavingsPrediction
            {
                PredictedSearchMinutes = searchTime,
                MinutesSavedPerSearch = minutesSaved,
                MonthlySavingsBRL = monthlySavingsBRL
            });
        }
    }
}
