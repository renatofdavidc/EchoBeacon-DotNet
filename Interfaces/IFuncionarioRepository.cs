using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Interfaces
{
    public interface IFuncionarioRepository
    {
        Task<PagedResult<Funcionario>> GetAllAsync(FuncionarioFilter filter);
        Task<Funcionario?> GetByIdAsync(int id);
        Task<Funcionario> AddAsync(Funcionario funcionario);
        Task<Funcionario?> UpdateAsync(int id, Funcionario funcionario);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
