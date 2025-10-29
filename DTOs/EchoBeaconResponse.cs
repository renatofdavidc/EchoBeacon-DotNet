namespace ProjetoChallengeMottu.DTOs
{
    public class EchoBeaconResponse
    {
        public int IdEchoBeacon { get; set; }
        public string CodigoIdentificador { get; set; } = string.Empty;
        public string StatusDispositivo { get; set; } = string.Empty;
        public string TipoSinal { get; set; } = string.Empty;
        public int RegistradaPor { get; set; }
        public string? NomeFuncionario { get; set; }
        public List<LinkDto> Links { get; set; } = new();
    }
}

