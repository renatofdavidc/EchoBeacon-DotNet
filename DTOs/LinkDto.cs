namespace ProjetoChallengeMottu.DTOs
{
    public class LinkDto
    {
        public string Rel { get; set; } = string.Empty;
        public string Href { get; set; } = string.Empty;
        public string Method { get; set; } = "GET";
        public string? Type { get; set; }
    }
}
