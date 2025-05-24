using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Interfaces
{
    public interface IEchoBeaconRepository
    {
        Task AddAsync(EchoBeacon entity);
        Task<EchoBeacon?> FindByIdAsync(long id);
        Task<EchoBeacon?> UpdateAsync(EchoBeacon entity);
        Task<bool> DeleteAsync(long id);
        IQueryable<EchoBeacon> QueryWithFilters(EchoBeaconFilter filter);
    }
}
