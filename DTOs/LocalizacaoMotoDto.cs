using System.ComponentModel.DataAnnotations;

namespace ProjetoChallengeMottu.DTOs
{
    public class LocalizacaoMotoRequest
    {
        [Required(ErrorMessage = "ID da localização é obrigatório")]
        public int IdLocalizacao { get; set; }

        [Required(ErrorMessage = "ID da moto é obrigatório")]
        public int IdMoto { get; set; }

        [StringLength(50, ErrorMessage = "Setor deve ter no máximo 50 caracteres")]
        public string? Setor { get; set; }

        [StringLength(10, ErrorMessage = "Vaga deve ter no máximo 10 caracteres")]
        public string? Vaga { get; set; }
    }

    public class LocalizacaoMotoResponse
    {
        public int IdLocalizacao { get; set; }
        public int IdMoto { get; set; }
        public string? Setor { get; set; }
        public string? Vaga { get; set; }
        public DateTime DataHoraRegistro { get; set; }
        public string? PlacaMoto { get; set; }
        public List<LinkDto> Links { get; set; } = new();
    }
}
