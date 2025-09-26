using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoChallengeMottu.Models
{
    [Table("LOCALIZACOES")]
    public class Localizacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long MotoId { get; set; }

        public long? EchoBeaconId { get; set; }

        [Required, StringLength(50)]
        public string Setor { get; set; } = "Recepcao";

        [Required]
        public LocalizacaoStatus Status { get; set; } = LocalizacaoStatus.Recebida;

        [Required]
        public DateTime DataHoraRegistro { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(MotoId))]
        public Moto? Moto { get; set; }

        [ForeignKey(nameof(EchoBeaconId))]
        public EchoBeacon? EchoBeacon { get; set; }
    }

    public enum LocalizacaoStatus
    {
        Recebida = 0,
        Patio = 1,
        EmReparo = 2,
        Finalizada = 3
    }
}
