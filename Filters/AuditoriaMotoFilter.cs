namespace ProjetoChallengeMottu.Filters
{
    public class AuditoriaMotoFilter
    {
        public string? Usuario { get; set; }
        public string? Operacao { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? Placa { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}
