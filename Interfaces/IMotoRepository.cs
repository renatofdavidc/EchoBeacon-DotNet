using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoChallengeMottu.Interfaces
{
    public interface IMotoRepository
    {
        IQueryable<Moto> QueryWithFilters(MotoFilter filter);
        Task<Moto?> FindByIdAsync(long id);
        Task AddAsync(Moto moto);
        Task<Moto?> UpdateAsync(Moto moto);
        Task<bool> DeleteAsync(long id);
    }
}
