namespace ProjetoChallengeMottu.DTOs
{
    public class MotoRequest
    {
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public long? EchoBeaconId { get; set; }
    }
}
