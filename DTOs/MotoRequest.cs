using System.ComponentModel.DataAnnotations;

namespace ProjetoChallengeMottu.DTOs
{
    public class MotoRequest
    {
        [Required(ErrorMessage = "ID da moto é obrigatório")]
        public int IdMoto { get; set; }

        [Required(ErrorMessage = "Placa é obrigatória")]
        [StringLength(10, ErrorMessage = "Placa deve ter no máximo 10 caracteres")]
        [RegularExpression(@"^[A-Z]{3}[0-9]{4}$", ErrorMessage = "Placa deve estar no formato ABC1234")]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chassi é obrigatório")]
        [StringLength(50, ErrorMessage = "Chassi deve ter no máximo 50 caracteres")]
        public string Chassi { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Problema deve ter no máximo 255 caracteres")]
        public string? Problema { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Custo de manutenção deve estar entre 0 e 999999.99")]
        public decimal? CustoManutencao { get; set; }

        public int? IdEchoBeacon { get; set; }
    }
}

