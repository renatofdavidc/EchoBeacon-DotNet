using System.ComponentModel.DataAnnotations;

namespace ProjetoChallengeMottu.DTOs
{
    public class FuncionarioRequest
    {
        [Required(ErrorMessage = "ID do funcionário é obrigatório")]
        public int IdFuncionario { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }

        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Telefone { get; set; }

        [StringLength(50, ErrorMessage = "Cargo deve ter no máximo 50 caracteres")]
        public string? Cargo { get; set; }
    }

    public class FuncionarioResponse
    {
        public int IdFuncionario { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Cargo { get; set; }
        public List<LinkDto> Links { get; set; } = new();
    }
}
