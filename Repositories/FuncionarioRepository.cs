using Microsoft.EntityFrameworkCore;
using ProjetoChallengeMottu.Data;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Repositories
{
    public class FuncionarioRepository : IFuncionarioRepository
    {
        private readonly ApplicationDbContext _context;

        public FuncionarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Funcionario>> GetAllAsync(FuncionarioFilter filter)
        {
            var query = _context.Funcionarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Nome))
                query = query.Where(f => EF.Functions.Like(f.Nome.ToUpper(), $"%{filter.Nome.ToUpper()}%"));

            if (!string.IsNullOrWhiteSpace(filter.Email))
                query = query.Where(f => EF.Functions.Like(f.Email!.ToUpper(), $"%{filter.Email.ToUpper()}%"));

            if (!string.IsNullOrWhiteSpace(filter.Cargo))
                query = query.Where(f => EF.Functions.Like(f.Cargo!.ToUpper(), $"%{filter.Cargo.ToUpper()}%"));

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(f => f.IdFuncionario)
                .Skip((filter.Page - 1) * filter.Size)
                .Take(filter.Size)
                .ToListAsync();

            return new PagedResult<Funcionario>
            {
                Items = items,
                TotalItems = totalItems,
                Page = filter.Page,
                Size = filter.Size
            };
        }

        public async Task<Funcionario?> GetByIdAsync(int id)
        {
            return await _context.Funcionarios.FindAsync(id);
        }

        public async Task<Funcionario> AddAsync(Funcionario funcionario)
        {
            _context.Funcionarios.Add(funcionario);
            await _context.SaveChangesAsync();
            return funcionario;
        }

        public async Task<Funcionario?> UpdateAsync(int id, Funcionario funcionario)
        {
            var existing = await _context.Funcionarios.FindAsync(id);
            if (existing == null) return null;

            existing.Nome = funcionario.Nome;
            existing.Email = funcionario.Email;
            existing.Telefone = funcionario.Telefone;
            existing.Cargo = funcionario.Cargo;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null) return false;

            _context.Funcionarios.Remove(funcionario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Funcionarios.AnyAsync(f => f.IdFuncionario == id);
        }
    }
}
