using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjetoChallengeMottu.Models
{
    [Table("EchoBeacon")]
    public class EchoBeacon
    {
        [Key]
        [Column("id_echo_beacon")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdEchoBeacon { get; set; }

        [Required]
        [StringLength(50)]
        [Column("codigo_identificador")]
        public string CodigoIdentificador { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("status_dispositivo")]
        public string StatusDispositivo { get; set; } = "ativo";

        [Required]
        [StringLength(20)]
        [Column("tipo_sinal")]
        public string TipoSinal { get; set; } = "buzzer_led";

        [Required]
        [Column("registrada_por")]
        public int RegistradaPor { get; set; }

        // Navegação
        [ForeignKey(nameof(RegistradaPor))]
        [JsonIgnore]
        public Funcionario? Funcionario { get; set; }

        [JsonIgnore]
        public Moto? Moto { get; set; }

        [JsonIgnore]
        public ICollection<LocalizacaoMoto>? LocalizacoesMoto { get; set; }
    }

    public enum StatusDispositivo
    {
        Ativo,
        Inativo,
        Manutencao
    }

    public enum TipoSinal
    {
        Buzzer,
        Led,
        BuzzerLed
    }
}
