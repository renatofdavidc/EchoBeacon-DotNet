using Microsoft.EntityFrameworkCore;
using ProjetoChallengeMottu.Data;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Repositories
{
    public class AuditoriaMotoRepository : IAuditoriaMotoRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditoriaMotoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<AuditoriaMoto>> GetAllAsync(AuditoriaMotoFilter filter)
        {
            var query = _context.AuditoriaMotos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Usuario))
                query = query.Where(a => EF.Functions.Like(a.Usuario!.ToUpper(), $"%{filter.Usuario.ToUpper()}%"));

            if (!string.IsNullOrWhiteSpace(filter.Operacao))
                query = query.Where(a => a.Operacao == filter.Operacao);

            if (filter.DataInicio.HasValue)
                query = query.Where(a => a.DataHora >= filter.DataInicio.Value);

            if (filter.DataFim.HasValue)
                query = query.Where(a => a.DataHora <= filter.DataFim.Value);

            if (!string.IsNullOrWhiteSpace(filter.Placa))
                query = query.Where(a => a.PlacaOld == filter.Placa || a.PlacaNew == filter.Placa);

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(a => a.DataHora)
                .Skip((filter.Page - 1) * filter.Size)
                .Take(filter.Size)
                .ToListAsync();

            return new PagedResult<AuditoriaMoto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = filter.Page,
                Size = filter.Size
            };
        }

        public async Task<AuditoriaMoto?> GetByIdAsync(int id)
        {
            return await _context.AuditoriaMotos.FindAsync(id);
        }
    }
}
