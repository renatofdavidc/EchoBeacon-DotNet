using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Interfaces
{
    public interface ILocalizacaoRepository
    {
        IQueryable<Localizacao> QueryWithFilters(LocalizacaoFilter filter);
        Task<Localizacao?> FindByIdAsync(long id);
        Task AddAsync(Localizacao entity);
        Task<Localizacao?> UpdateAsync(Localizacao entity);
        Task<bool> DeleteAsync(long id);
    }
}
