using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjetoChallengeMottu.Models
{
    [Table("MOTOS")]
    public class Moto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required, StringLength(10)]
        public string Placa { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Modelo { get; set; } = string.Empty;

        [JsonIgnore]
        public EchoBeacon? EchoBeacon { get; set; }
    }
}
