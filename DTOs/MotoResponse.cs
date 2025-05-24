namespace ProjetoChallengeMottu.DTOs
{
    public class MotoResponse
    {
        public long Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public EchoBeaconResponse? EchoBeacon { get; set; }
    }
}