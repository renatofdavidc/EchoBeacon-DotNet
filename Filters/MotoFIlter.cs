namespace ProjetoChallengeMottu.Filters
{
    public class MotoFilter
    {
        public string? Placa { get; set; }
        public string? Chassi { get; set; }
        public string? Problema { get; set; }
        public decimal? CustoManutencaoMin { get; set; }
        public decimal? CustoManutencaoMax { get; set; }
        public int? IdEchoBeacon { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}

