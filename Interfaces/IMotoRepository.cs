using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Interfaces
{
    public interface IMotoRepository
    {
        Task<PagedResult<Moto>> GetAllAsync(MotoFilter filter);
        Task<Moto?> GetByIdAsync(int id);
        Task<Moto> AddAsync(Moto moto);
        Task<Moto?> UpdateAsync(int id, Moto moto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

