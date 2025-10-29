using Microsoft.AspNetCore.Mvc;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/localizacoes")]
    [ApiVersion("1.0")]
    public class LocalizacoesMotoController : ControllerBase
    {
        private readonly ILocalizacaoMotoRepository _repository;
        private readonly IMotoRepository _motos;

        public LocalizacoesMotoController(ILocalizacaoMotoRepository repository, IMotoRepository motos)
        {
            _repository = repository;
            _motos = motos;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<LocalizacaoMotoResponse>>> GetAll([FromQuery] LocalizacaoMotoFilter filter)
        {
            var result = await _repository.GetAllAsync(filter);
            var items = result.Items.Select(l => new LocalizacaoMotoResponse
            {
                IdLocalizacao = l.IdLocalizacao,
                IdMoto = l.IdMoto,
                Setor = l.Setor,
                Vaga = l.Vaga,
                DataHoraRegistro = l.DataHoraRegistro
            }).ToList();

            return Ok(new PagedResult<LocalizacaoMotoResponse>
            {
                Items = items,
                Page = result.Page,
                Size = result.Size,
                TotalItems = result.TotalItems
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LocalizacaoMotoResponse>> GetById(int id)
        {
            var l = await _repository.GetByIdAsync(id);
            if (l == null) return NotFound();

            return Ok(new LocalizacaoMotoResponse
            {
                IdLocalizacao = l.IdLocalizacao,
                IdMoto = l.IdMoto,
                Setor = l.Setor,
                Vaga = l.Vaga,
                DataHoraRegistro = l.DataHoraRegistro
            });
        }

        [HttpPost]
        public async Task<ActionResult<LocalizacaoMotoResponse>> Create([FromBody] LocalizacaoMotoRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var motoExists = await _motos.ExistsAsync(request.IdMoto);
            if (!motoExists) return BadRequest("Moto não encontrada");

            var entity = new LocalizacaoMoto
            {
                IdLocalizacao = request.IdLocalizacao,
                IdMoto = request.IdMoto,
                Setor = request.Setor,
                Vaga = request.Vaga
            };
            var created = await _repository.AddAsync(entity);

            var response = new LocalizacaoMotoResponse
            {
                IdLocalizacao = created.IdLocalizacao,
                IdMoto = created.IdMoto,
                Setor = created.Setor,
                Vaga = created.Vaga,
                DataHoraRegistro = created.DataHoraRegistro
            };

            return CreatedAtAction(nameof(GetById), new { id = response.IdLocalizacao }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LocalizacaoMotoRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new LocalizacaoMoto
            {
                IdLocalizacao = request.IdLocalizacao,
                IdMoto = request.IdMoto,
                Setor = request.Setor,
                Vaga = request.Vaga,
                DataHoraRegistro = DateTime.Now
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
