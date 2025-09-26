namespace ProjetoChallengeMottu.DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<LinkDto> Links { get; set; } = Array.Empty<LinkDto>();
    }
}
