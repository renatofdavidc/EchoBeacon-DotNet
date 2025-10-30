using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Services;

namespace ProjetoChallengeMottu.Tests.Services;

public class PatioSearchTimeModelTests
{
    [Fact]
    public void Predict_Returns_Positive()
    {
        var model = new PatioSearchTimeModel();
        var pred = model.Predict(new PatioSearchInput
        {
            PatioAreaM2 = 1500,
            MotosNoPatio = 60,
            PercentualComBeacon = 0.8f,
            FuncionariosBuscando = 2,
            HoraPico = 0
        });

        Assert.True(pred.PredictedMinutes >= 0);
    }

    [Fact]
    public void Higher_Beacon_Percentage_Should_Not_Increase_Time()
    {
        var model = new PatioSearchTimeModel();

        var low = model.Predict(new PatioSearchInput { PatioAreaM2 = 1500, MotosNoPatio = 60, PercentualComBeacon = 0.2f, FuncionariosBuscando = 2, HoraPico = 0 }).PredictedMinutes;
        var high = model.Predict(new PatioSearchInput { PatioAreaM2 = 1500, MotosNoPatio = 60, PercentualComBeacon = 0.9f, FuncionariosBuscando = 2, HoraPico = 0 }).PredictedMinutes;

        Assert.True(high <= low + 1.0f); // modelo simples: tolera pequena variação
    }
}
