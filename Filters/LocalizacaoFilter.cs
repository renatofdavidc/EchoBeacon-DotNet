using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Filters
{
    public class LocalizacaoFilter
    {
        public long? MotoId { get; set; }
        public long? EchoBeaconId { get; set; }
        public string? Setor { get; set; }
        public LocalizacaoStatus? Status { get; set; }
    }
}
