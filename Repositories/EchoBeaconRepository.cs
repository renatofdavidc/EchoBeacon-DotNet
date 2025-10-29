using Microsoft.EntityFrameworkCore;
using ProjetoChallengeMottu.Data;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Repositories
{
    public class EchoBeaconRepository : IEchoBeaconRepository
    {
        private readonly ApplicationDbContext _context;

        public EchoBeaconRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<EchoBeacon>> GetAllAsync(EchoBeaconFilter filter)
        {
            var query = _context.EchoBeacons
                .Include(e => e.Funcionario)
                .Include(e => e.Moto)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.CodigoIdentificador))
                query = query.Where(e => EF.Functions.Like(e.CodigoIdentificador.ToUpper(), $"%{filter.CodigoIdentificador.ToUpper()}%"));

            if (!string.IsNullOrWhiteSpace(filter.StatusDispositivo))
                query = query.Where(e => e.StatusDispositivo == filter.StatusDispositivo);

            if (!string.IsNullOrWhiteSpace(filter.TipoSinal))
                query = query.Where(e => e.TipoSinal == filter.TipoSinal);

            if (filter.RegistradaPor.HasValue)
                query = query.Where(e => e.RegistradaPor == filter.RegistradaPor.Value);

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.IdEchoBeacon)
                .Skip((filter.Page - 1) * filter.Size)
                .Take(filter.Size)
                .ToListAsync();

            return new PagedResult<EchoBeacon>
            {
                Items = items,
                TotalItems = totalItems,
                Page = filter.Page,
                Size = filter.Size
            };
        }

        public async Task<EchoBeacon?> GetByIdAsync(int id)
        {
            return await _context.EchoBeacons
                .Include(e => e.Funcionario)
                .Include(e => e.Moto)
                .FirstOrDefaultAsync(e => e.IdEchoBeacon == id);
        }

        public async Task<EchoBeacon> AddAsync(EchoBeacon echoBeacon)
        {
            _context.EchoBeacons.Add(echoBeacon);
            await _context.SaveChangesAsync();
            return echoBeacon;
        }

        public async Task<EchoBeacon?> UpdateAsync(int id, EchoBeacon echoBeacon)
        {
            var existing = await _context.EchoBeacons.FindAsync(id);
            if (existing == null) return null;

            existing.CodigoIdentificador = echoBeacon.CodigoIdentificador;
            existing.StatusDispositivo = echoBeacon.StatusDispositivo;
            existing.TipoSinal = echoBeacon.TipoSinal;
            existing.RegistradaPor = echoBeacon.RegistradaPor;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var echoBeacon = await _context.EchoBeacons.FindAsync(id);
            if (echoBeacon == null) return false;

            _context.EchoBeacons.Remove(echoBeacon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.EchoBeacons.AnyAsync(e => e.IdEchoBeacon == id);
        }
    }
}


