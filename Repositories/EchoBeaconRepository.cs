using ProjetoChallengeMottu.Data;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjetoChallengeMottu.Repositories
{
    public class EchoBeaconRepository : IEchoBeaconRepository
    {
        private readonly ApplicationDbContext _context;

        public EchoBeaconRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EchoBeacon entity)
        {
            await _context.EchoBeacons.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<EchoBeacon?> FindByIdAsync(long id)
        {
            return await _context.EchoBeacons.FindAsync(id);
        }

        public async Task<EchoBeacon?> UpdateAsync(EchoBeacon entity)
        {
            var existing = await _context.EchoBeacons.FindAsync(entity.Id);
            if (existing == null)
                return null;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return existing;
        }


        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.EchoBeacons.FindAsync(id);
            if (entity == null) return false;

            _context.EchoBeacons.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public IQueryable<EchoBeacon> QueryWithFilters(EchoBeaconFilter filter)
        {
            var query = _context.EchoBeacons.Include(e => e.Moto).AsQueryable();
            if (!string.IsNullOrEmpty(filter.NumeroIdentificacao))
                query = query.Where(e => e.NumeroIdentificacao == filter.NumeroIdentificacao);
            return query;
        }

    }
}
