namespace ProjetoChallengeMottu.DTOs
{
    public class PatioSearchInput
    {
        // Área total do pátio em m²
        public float PatioAreaM2 { get; set; }

        // Número de motos atualmente no pátio
        public float MotosNoPatio { get; set; }

        // Fração de motos com EchoBeacon ativo (0..1)
        public float PercentualComBeacon { get; set; }

        // Número de funcionários que podem buscar simultaneamente
        public float FuncionariosBuscando { get; set; }

        // 1 se for horário de pico, 0 caso contrário
        public float HoraPico { get; set; }
    }
}
