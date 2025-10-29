namespace ProjetoChallengeMottu.Filters
{
    public class EchoBeaconFilter
    {
        public string? CodigoIdentificador { get; set; }
        public string? StatusDispositivo { get; set; }
        public string? TipoSinal { get; set; }
        public int? RegistradaPor { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}

