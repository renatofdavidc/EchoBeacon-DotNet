using Microsoft.AspNetCore.Mvc;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/funcionarios")]
    [ApiVersion("1.0")]
    public class FuncionariosController : ControllerBase
    {
        private readonly IFuncionarioRepository _repository;

        public FuncionariosController(IFuncionarioRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<FuncionarioResponse>>> GetAll([FromQuery] FuncionarioFilter filter)
        {
            var result = await _repository.GetAllAsync(filter);
            var items = result.Items.Select(f => new FuncionarioResponse
            {
                IdFuncionario = f.IdFuncionario,
                Nome = f.Nome,
                Email = f.Email,
                Telefone = f.Telefone,
                Cargo = f.Cargo
            }).ToList();

            return Ok(new PagedResult<FuncionarioResponse>
            {
                Items = items,
                Page = result.Page,
                Size = result.Size,
                TotalItems = result.TotalItems
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FuncionarioResponse>> GetById(int id)
        {
            var f = await _repository.GetByIdAsync(id);
            if (f == null) return NotFound();

            return Ok(new FuncionarioResponse
            {
                IdFuncionario = f.IdFuncionario,
                Nome = f.Nome,
                Email = f.Email,
                Telefone = f.Telefone,
                Cargo = f.Cargo
            });
        }

        [HttpPost]
        public async Task<ActionResult<FuncionarioResponse>> Create([FromBody] FuncionarioRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new Funcionario
            {
                IdFuncionario = request.IdFuncionario,
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone,
                Cargo = request.Cargo
            };
            var created = await _repository.AddAsync(entity);

            var response = new FuncionarioResponse
            {
                IdFuncionario = created.IdFuncionario,
                Nome = created.Nome,
                Email = created.Email,
                Telefone = created.Telefone,
                Cargo = created.Cargo
            };

            return CreatedAtAction(nameof(GetById), new { id = response.IdFuncionario }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FuncionarioRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new Funcionario
            {
                IdFuncionario = request.IdFuncionario,
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone,
                Cargo = request.Cargo
            };

            var updated = await _repository.UpdateAsync(id, entity);
            if (updated == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _repository.DeleteAsync(id);
            if (!removed) return NotFound();
            return NoContent();
        }
    }
}
