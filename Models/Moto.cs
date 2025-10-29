using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjetoChallengeMottu.Models
{
    [Table("Moto")]
    public class Moto
    {
        [Key]
        [Column("id_moto")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdMoto { get; set; }

        [Required]
        [StringLength(10)]
        [Column("placa")]
        public string Placa { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("chassi")]
        public string Chassi { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("problema")]
        public string? Problema { get; set; }

        [Column("custo_manutencao", TypeName = "NUMBER(10,2)")]
        public decimal? CustoManutencao { get; set; }

        [Column("id_echo_beacon")]
        public int? IdEchoBeacon { get; set; }

        // Navegação
        [ForeignKey(nameof(IdEchoBeacon))]
        [JsonIgnore]
        public EchoBeacon? EchoBeacon { get; set; }

        [JsonIgnore]
        public ICollection<LocalizacaoMoto>? LocalizacoesMoto { get; set; }

        [JsonIgnore]
        public ICollection<AuditoriaMoto>? Auditorias { get; set; }
    }
}
