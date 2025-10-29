using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Interfaces
{
    public interface IAuditoriaMotoRepository
    {
        Task<PagedResult<AuditoriaMoto>> GetAllAsync(AuditoriaMotoFilter filter);
        Task<AuditoriaMoto?> GetByIdAsync(int id);
    }
}
