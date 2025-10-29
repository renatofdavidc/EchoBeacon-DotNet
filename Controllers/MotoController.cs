using Microsoft.AspNetCore.Mvc;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/motos")]
    [ApiVersion("1.0")]
    public class MotosController : ControllerBase
    {
        private readonly IMotoRepository _motoRepository;
        private readonly IEchoBeaconRepository _echoBeaconRepository;

        public MotosController(IMotoRepository motoRepository, IEchoBeaconRepository echoBeaconRepository)
        {
            _motoRepository = motoRepository;
            _echoBeaconRepository = echoBeaconRepository;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<MotoResponse>>> GetAll([FromQuery] MotoFilter filter)
        {
            var result = await _motoRepository.GetAllAsync(filter);
            var items = result.Items.Select(m => new MotoResponse
            {
                IdMoto = m.IdMoto,
                Placa = m.Placa,
                Chassi = m.Chassi,
                Problema = m.Problema,
                CustoManutencao = m.CustoManutencao,
                IdEchoBeacon = m.IdEchoBeacon,
                CodigoEchoBeacon = m.EchoBeacon?.CodigoIdentificador
            }).ToList();

            return Ok(new PagedResult<MotoResponse>
            {
                Items = items,
                Page = result.Page,
                Size = result.Size,
                TotalItems = result.TotalItems
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MotoResponse>> GetById(int id)
        {
            var m = await _motoRepository.GetByIdAsync(id);
            if (m == null) return NotFound();

            return Ok(new MotoResponse
            {
                IdMoto = m.IdMoto,
                Placa = m.Placa,
                Chassi = m.Chassi,
                Problema = m.Problema,
                CustoManutencao = m.CustoManutencao,
                IdEchoBeacon = m.IdEchoBeacon,
                CodigoEchoBeacon = m.EchoBeacon?.CodigoIdentificador
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MotoRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (request.IdEchoBeacon.HasValue)
            {
                var exists = await _echoBeaconRepository.ExistsAsync(request.IdEchoBeacon.Value);
                if (!exists) return BadRequest("EchoBeacon não encontrada");
            }

            var moto = new Moto
            {
                IdMoto = request.IdMoto,
                Placa = request.Placa,
                Chassi = request.Chassi,
                Problema = request.Problema,
                CustoManutencao = request.CustoManutencao,
                IdEchoBeacon = request.IdEchoBeacon
            };

            var created = await _motoRepository.AddAsync(moto);

            var dto = new MotoResponse
            {
                IdMoto = created.IdMoto,
                Placa = created.Placa,
                Chassi = created.Chassi,
                Problema = created.Problema,
                CustoManutencao = created.CustoManutencao,
                IdEchoBeacon = created.IdEchoBeacon,
                CodigoEchoBeacon = created.EchoBeacon?.CodigoIdentificador
            };

            return CreatedAtAction(nameof(GetById), new { id = created.IdMoto }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MotoRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var toUpdate = new Moto
            {
                IdMoto = request.IdMoto,
                Placa = request.Placa,
                Chassi = request.Chassi,
                Problema = request.Problema,
                CustoManutencao = request.CustoManutencao,
                IdEchoBeacon = request.IdEchoBeacon
            };

            var updated = await _motoRepository.UpdateAsync(id, toUpdate);
            if (updated == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _motoRepository.DeleteAsync(id);
            if (!removed) return NotFound();
            return NoContent();
        }
    }
}
