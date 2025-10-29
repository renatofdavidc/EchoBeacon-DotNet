namespace ProjetoChallengeMottu.DTOs
{
    public class MotoResponse
    {
        public int IdMoto { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Chassi { get; set; } = string.Empty;
        public string? Problema { get; set; }
        public decimal? CustoManutencao { get; set; }
        public int? IdEchoBeacon { get; set; }
        public string? CodigoEchoBeacon { get; set; }
        public List<LinkDto> Links { get; set; } = new();
    }
}
