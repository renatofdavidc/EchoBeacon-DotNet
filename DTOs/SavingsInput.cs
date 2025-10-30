namespace ProjetoChallengeMottu.DTOs
{
    public class SavingsInput : PatioSearchInput
    {
        // Tempo médio atual (sem o sistema) para localizar uma moto, em minutos
        public float BaselineTimePerSearchMin { get; set; }

        // Quantidade média de buscas por dia
        public float SearchesPerDay { get; set; }

        // Custo/hora da equipe de busca (R$)
        public float HourlyCostBRL { get; set; }
    }
}
