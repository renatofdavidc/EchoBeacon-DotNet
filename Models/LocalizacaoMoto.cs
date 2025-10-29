using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjetoChallengeMottu.Models
{
    [Table("LocalizacaoMoto")]
    public class LocalizacaoMoto
    {
        [Key]
        [Column("id_localizacao")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdLocalizacao { get; set; }

        [Required]
        [Column("id_moto")]
        public int IdMoto { get; set; }

        [StringLength(50)]
        [Column("setor")]
        public string? Setor { get; set; }

        [StringLength(10)]
        [Column("vaga")]
        public string? Vaga { get; set; }

        [Column("data_hora_registro")]
        public DateTime DataHoraRegistro { get; set; } = DateTime.Now;

        // Navegação
        [ForeignKey(nameof(IdMoto))]
        [JsonIgnore]
        public Moto? Moto { get; set; }
    }
}
