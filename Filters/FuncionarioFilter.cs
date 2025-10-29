namespace ProjetoChallengeMottu.Filters
{
    public class FuncionarioFilter
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Cargo { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}
