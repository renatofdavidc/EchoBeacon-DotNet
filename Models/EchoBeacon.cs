using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjetoChallengeMottu.Models
{
    [Table("ECHOBEACON")]
    public class EchoBeacon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required, StringLength(10)]
        public string NumeroIdentificacao { get; set; } = string.Empty;

        [Required]
        public DateTime DataRegistro { get; set; }

        public long? MotoId { get; set; }

        [ForeignKey(nameof(MotoId))]
        [JsonIgnore]
        public Moto? Moto { get; set; }
    }
}
