using System.ComponentModel.DataAnnotations;

namespace ProjetoChallengeMottu.DTOs
{
    public class EchoBeaconRequest
    {
        [Required(ErrorMessage = "ID do EchoBeacon é obrigatório")]
        public int IdEchoBeacon { get; set; }

        [Required(ErrorMessage = "Código identificador é obrigatório")]
        [StringLength(50, ErrorMessage = "Código identificador deve ter no máximo 50 caracteres")]
        public string CodigoIdentificador { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status do dispositivo é obrigatório")]
        [RegularExpression("^(ativo|inativo|manutencao)$", ErrorMessage = "Status deve ser: ativo, inativo ou manutencao")]
        public string StatusDispositivo { get; set; } = "ativo";

        [Required(ErrorMessage = "Tipo de sinal é obrigatório")]
        [RegularExpression("^(buzzer|led|buzzer_led)$", ErrorMessage = "Tipo de sinal deve ser: buzzer, led ou buzzer_led")]
        public string TipoSinal { get; set; } = "buzzer_led";

        [Required(ErrorMessage = "ID do funcionário que registrou é obrigatório")]
        public int RegistradaPor { get; set; }
    }
}

