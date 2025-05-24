using Microsoft.EntityFrameworkCore;
using ProjetoChallengeMottu.Data;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoChallengeMottu.Repositories
{
    public class MotoRepository : IMotoRepository
    {
        private readonly ApplicationDbContext _context;

        public MotoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Moto> QueryWithFilters(MotoFilter filter)
        {
            // Retorna IQueryable<Moto> para permitir Include, paginação etc.
            var query = _context.Motos
                .Include(m => m.EchoBeacon)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Placa))
                query = query.Where(m => m.Placa.Contains(filter.Placa));

            if (!string.IsNullOrEmpty(filter.Modelo))
                query = query.Where(m => m.Modelo.Contains(filter.Modelo));

            return query;
        }

        public async Task<Moto?> FindByIdAsync(long id)
        {
            return await _context.Motos.FindAsync(id);
        }

        public async Task AddAsync(Moto moto)
        {
            await _context.Motos.AddAsync(moto);
            await _context.SaveChangesAsync();
        }

        public async Task<Moto?> UpdateAsync(Moto moto)
        {
            var existing = await _context.Motos.FindAsync(moto.Id);
            if (existing == null)
                return null;

            _context.Entry(existing).CurrentValues.SetValues(moto);
            await _context.SaveChangesAsync();
            return existing;
        }


        public async Task<bool> DeleteAsync(long id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return false;

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
