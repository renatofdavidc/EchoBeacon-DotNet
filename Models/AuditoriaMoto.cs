using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoChallengeMottu.Models
{
    [Table("AuditoriaMoto")]
    public class AuditoriaMoto
    {
        [Key]
        [Column("id_auditoria")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAuditoria { get; set; }

        [StringLength(50)]
        [Column("usuario")]
        public string? Usuario { get; set; }

        [StringLength(20)]
        [Column("operacao")]
        public string? Operacao { get; set; }

        [Column("data_hora")]
        public DateTime? DataHora { get; set; }

        [StringLength(10)]
        [Column("placa_old")]
        public string? PlacaOld { get; set; }

        [StringLength(10)]
        [Column("placa_new")]
        public string? PlacaNew { get; set; }

        [StringLength(255)]
        [Column("problema_old")]
        public string? ProblemaOld { get; set; }

        [StringLength(255)]
        [Column("problema_new")]
        public string? ProblemaNew { get; set; }
    }
}
