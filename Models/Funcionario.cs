using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoChallengeMottu.Models
{
    [Table("Funcionario")]
    public class Funcionario
    {
        [Key]
        [Column("id_funcionario")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdFuncionario { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(100)]
        [Column("email")]
        public string? Email { get; set; }

        [StringLength(20)]
        [Column("telefone")]
        public string? Telefone { get; set; }

        [StringLength(50)]
        [Column("cargo")]
        public string? Cargo { get; set; }

        // Navegação
        public ICollection<EchoBeacon>? EchoBeacons { get; set; }
    }
}
