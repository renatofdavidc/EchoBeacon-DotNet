namespace ProjetoChallengeMottu.DTOs
{
    public class MotoRequest
    {
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;

        // novo campo opcional para v�nculo
        public long? EchoBeaconId { get; set; }
    }
}
