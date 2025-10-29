using Microsoft.EntityFrameworkCore;
using ProjetoChallengeMottu.Data;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Repositories
{
    public class LocalizacaoMotoRepository : ILocalizacaoMotoRepository
    {
        private readonly ApplicationDbContext _context;

        public LocalizacaoMotoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<LocalizacaoMoto>> GetAllAsync(LocalizacaoMotoFilter filter)
        {
            var query = _context.LocalizacoesMoto.Include(l => l.Moto).AsQueryable();

            if (filter.IdMoto.HasValue)
                query = query.Where(l => l.IdMoto == filter.IdMoto.Value);

            if (!string.IsNullOrWhiteSpace(filter.Setor))
                query = query.Where(l => EF.Functions.Like(l.Setor!.ToUpper(), $"%{filter.Setor.ToUpper()}%"));

            if (!string.IsNullOrWhiteSpace(filter.Vaga))
                query = query.Where(l => EF.Functions.Like(l.Vaga!.ToUpper(), $"%{filter.Vaga.ToUpper()}%"));

            if (filter.DataInicio.HasValue)
                query = query.Where(l => l.DataHoraRegistro >= filter.DataInicio.Value);

            if (filter.DataFim.HasValue)
                query = query.Where(l => l.DataHoraRegistro <= filter.DataFim.Value);

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(l => l.DataHoraRegistro)
                .Skip((filter.Page - 1) * filter.Size)
                .Take(filter.Size)
                .ToListAsync();

            return new PagedResult<LocalizacaoMoto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = filter.Page,
                Size = filter.Size
            };
        }

        public async Task<LocalizacaoMoto?> GetByIdAsync(int id)
        {
            return await _context.LocalizacoesMoto.Include(l => l.Moto).FirstOrDefaultAsync(l => l.IdLocalizacao == id);
        }

        public async Task<LocalizacaoMoto> AddAsync(LocalizacaoMoto localizacao)
        {
            _context.LocalizacoesMoto.Add(localizacao);
            await _context.SaveChangesAsync();
            return localizacao;
        }

        public async Task<LocalizacaoMoto?> UpdateAsync(int id, LocalizacaoMoto localizacao)
        {
            var existing = await _context.LocalizacoesMoto.FindAsync(id);
            if (existing == null) return null;

            existing.IdMoto = localizacao.IdMoto;
            existing.Setor = localizacao.Setor;
            existing.Vaga = localizacao.Vaga;
            existing.DataHoraRegistro = localizacao.DataHoraRegistro;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var localizacao = await _context.LocalizacoesMoto.FindAsync(id);
            if (localizacao == null) return false;

            _context.LocalizacoesMoto.Remove(localizacao);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.LocalizacoesMoto.AnyAsync(l => l.IdLocalizacao == id);
        }
    }
}
