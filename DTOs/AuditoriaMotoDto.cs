namespace ProjetoChallengeMottu.DTOs
{
    public class AuditoriaMotoResponse
    {
        public int IdAuditoria { get; set; }
        public string? Usuario { get; set; }
        public string? Operacao { get; set; }
        public DateTime? DataHora { get; set; }
        public string? PlacaOld { get; set; }
        public string? PlacaNew { get; set; }
        public string? ProblemaOld { get; set; }
        public string? ProblemaNew { get; set; }
        public List<LinkDto> Links { get; set; } = new();
    }
}
