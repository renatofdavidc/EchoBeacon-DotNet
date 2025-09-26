using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.DTOs
{
    public class LocalizacaoRequest
    {
        public long MotoId { get; set; }
        public long? EchoBeaconId { get; set; }
        public string Setor { get; set; } = string.Empty;
        public LocalizacaoStatus Status { get; set; } = LocalizacaoStatus.Recebida;
    }
}
