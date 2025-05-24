namespace ProjetoChallengeMottu.DTOs
{
    public class EchoBeaconRequest
    {
        public string NumeroIdentificacao { get; set; } = string.Empty;
        public DateTime DataRegistro { get; set; }
        public long? MotoId { get; set; }
    }
}
