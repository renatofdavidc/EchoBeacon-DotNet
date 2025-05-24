namespace ProjetoChallengeMottu.DTOs
{
    public class EchoBeaconResponse
    {
        public long Id { get; set; }
        public string NumeroIdentificacao { get; set; } = string.Empty;
        public DateTime DataRegistro { get; set; }
        public MotoResponse? Moto { get; set; }
    }
}
