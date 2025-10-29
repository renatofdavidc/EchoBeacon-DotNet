namespace ProjetoChallengeMottu.Filters
{
    public class LocalizacaoMotoFilter
    {
        public int? IdMoto { get; set; }
        public string? Setor { get; set; }
        public string? Vaga { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}
