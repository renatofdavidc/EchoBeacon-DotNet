using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Interfaces
{
    public interface IEchoBeaconRepository
    {
        Task<PagedResult<EchoBeacon>> GetAllAsync(EchoBeaconFilter filter);
        Task<EchoBeacon?> GetByIdAsync(int id);
        Task<EchoBeacon> AddAsync(EchoBeacon echoBeacon);
        Task<EchoBeacon?> UpdateAsync(int id, EchoBeacon echoBeacon);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

