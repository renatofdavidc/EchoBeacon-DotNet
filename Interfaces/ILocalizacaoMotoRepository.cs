using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Interfaces
{
    public interface ILocalizacaoMotoRepository
    {
        Task<PagedResult<LocalizacaoMoto>> GetAllAsync(LocalizacaoMotoFilter filter);
        Task<LocalizacaoMoto?> GetByIdAsync(int id);
        Task<LocalizacaoMoto> AddAsync(LocalizacaoMoto localizacao);
        Task<LocalizacaoMoto?> UpdateAsync(int id, LocalizacaoMoto localizacao);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
