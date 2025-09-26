using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.DTOs
{
    public class LocalizacaoResponse
    {
        public long Id { get; set; }
        public string Setor { get; set; } = string.Empty;
        public LocalizacaoStatus Status { get; set; }
        public DateTime DataHoraRegistro { get; set; }
        public MotoResponse? Moto { get; set; }
        public EchoBeaconResponse? EchoBeacon { get; set; }
    }
}
