namespace ProjetoChallengeMottu.DTOs
{
    public class DeliveryTimeInput
    {
        /// <summary>
        /// Distância estimada do trajeto em quilômetros.
        /// </summary>
        public float DistanceKm { get; set; }

        /// <summary>
        /// Nível de trânsito: 0=baixo, 1=médio, 2=alto.
        /// </summary>
        public float TrafficLevel { get; set; }

        /// <summary>
        /// Quantidade de paradas previstas (semáforos/cancelas etc.).
        /// </summary>
        public float Stops { get; set; }
    }
}
