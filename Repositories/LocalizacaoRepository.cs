using Microsoft.EntityFrameworkCore;
using ProjetoChallengeMottu.Data;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Repositories
{
    public class LocalizacaoRepository : ILocalizacaoRepository
    {
        private readonly ApplicationDbContext _context;

        public LocalizacaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Localizacao> QueryWithFilters(LocalizacaoFilter filter)
        {
            var query = _context.Localizacoes
                .Include(l => l.Moto)
                .Include(l => l.EchoBeacon)
                .AsQueryable();

            if (filter.MotoId.HasValue)
                query = query.Where(l => l.MotoId == filter.MotoId);
            if (filter.EchoBeaconId.HasValue)
                query = query.Where(l => l.EchoBeaconId == filter.EchoBeaconId);
            if (!string.IsNullOrWhiteSpace(filter.Setor))
                query = query.Where(l => l.Setor.Contains(filter.Setor));
            if (filter.Status.HasValue)
                query = query.Where(l => l.Status == filter.Status);

            return query;
        }

        public async Task<Localizacao?> FindByIdAsync(long id)
        {
            return await _context.Localizacoes
                .Include(l => l.Moto)
                .Include(l => l.EchoBeacon)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task AddAsync(Localizacao entity)
        {
            await _context.Localizacoes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Localizacao?> UpdateAsync(Localizacao entity)
        {
            var existing = await _context.Localizacoes.FindAsync(entity.Id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var existing = await _context.Localizacoes.FindAsync(id);
            if (existing == null) return false;
            _context.Localizacoes.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
