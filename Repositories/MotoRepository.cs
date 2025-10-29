using Microsoft.EntityFrameworkCore;
using ProjetoChallengeMottu.Data;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Repositories
{
    public class MotoRepository : IMotoRepository
    {
        private readonly ApplicationDbContext _context;

        public MotoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Moto>> GetAllAsync(MotoFilter filter)
        {
            var query = _context.Motos.Include(m => m.EchoBeacon).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Placa))
                query = query.Where(m => EF.Functions.Like(m.Placa.ToUpper(), $"%{filter.Placa.ToUpper()}%"));

            if (!string.IsNullOrWhiteSpace(filter.Chassi))
                query = query.Where(m => EF.Functions.Like(m.Chassi.ToUpper(), $"%{filter.Chassi.ToUpper()}%"));

            if (!string.IsNullOrWhiteSpace(filter.Problema))
                query = query.Where(m => EF.Functions.Like(m.Problema!.ToUpper(), $"%{filter.Problema.ToUpper()}%"));

            if (filter.CustoManutencaoMin.HasValue)
                query = query.Where(m => m.CustoManutencao >= filter.CustoManutencaoMin.Value);

            if (filter.CustoManutencaoMax.HasValue)
                query = query.Where(m => m.CustoManutencao <= filter.CustoManutencaoMax.Value);

            if (filter.IdEchoBeacon.HasValue)
                query = query.Where(m => m.IdEchoBeacon == filter.IdEchoBeacon.Value);

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(m => m.IdMoto)
                .Skip((filter.Page - 1) * filter.Size)
                .Take(filter.Size)
                .ToListAsync();

            return new PagedResult<Moto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = filter.Page,
                Size = filter.Size
            };
        }

        public async Task<Moto?> GetByIdAsync(int id)
        {
            return await _context.Motos.Include(m => m.EchoBeacon).FirstOrDefaultAsync(m => m.IdMoto == id);
        }

        public async Task<Moto> AddAsync(Moto moto)
        {
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();
            return moto;
        }

        public async Task<Moto?> UpdateAsync(int id, Moto moto)
        {
            var existing = await _context.Motos.FindAsync(id);
            if (existing == null) return null;

            existing.Placa = moto.Placa;
            existing.Chassi = moto.Chassi;
            existing.Problema = moto.Problema;
            existing.CustoManutencao = moto.CustoManutencao;
            existing.IdEchoBeacon = moto.IdEchoBeacon;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return false;

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Motos.AnyAsync(m => m.IdMoto == id);
        }
    }
}

