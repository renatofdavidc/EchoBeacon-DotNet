using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoChallengeMottu.Controllers
{
    [ApiController]
    [Route("api/motos")]
    public class MotosController : ControllerBase
    {
        private readonly ILogger<MotosController> _logger;
        private readonly IMotoRepository _motoRepo;
        private readonly IEchoBeaconRepository _beaconRepo;

        public MotosController(
            ILogger<MotosController> logger,
            IMotoRepository motoRepo,
            IEchoBeaconRepository beaconRepo)
        {
            _logger = logger;
            _motoRepo = motoRepo;
            _beaconRepo = beaconRepo;
        }

        // GET: api/motos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MotoResponse>>> GetAll(
            [FromQuery] MotoFilter filter,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var motos = await _motoRepo
                .QueryWithFilters(filter)
                .Include(m => m.EchoBeacon)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var dtos = motos.Select(m => new MotoResponse
            {
                Id = m.Id,
                Placa = m.Placa,
                Modelo = m.Modelo,
                EchoBeacon = m.EchoBeacon is null
                    ? null
                    : new EchoBeaconResponse
                    {
                        Id = m.EchoBeacon.Id,
                        NumeroIdentificacao = m.EchoBeacon.NumeroIdentificacao,
                        DataRegistro = m.EchoBeacon.DataRegistro,
                        Moto = null
                    }
            });

            return Ok(dtos);
        }

        // GET: api/motos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MotoResponse>> GetById(long id)
        {
            var m = await _motoRepo
                .QueryWithFilters(new MotoFilter())
                .Include(m => m.EchoBeacon)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (m == null)
                return NotFound();

            var dto = new MotoResponse
            {
                Id = m.Id,
                Placa = m.Placa,
                Modelo = m.Modelo,
                EchoBeacon = m.EchoBeacon is null
                    ? null
                    : new EchoBeaconResponse
                    {
                        Id = m.EchoBeacon.Id,
                        NumeroIdentificacao = m.EchoBeacon.NumeroIdentificacao,
                        DataRegistro = m.EchoBeacon.DataRegistro,
                        Moto = null
                    }
            };

            return Ok(dto);
        }

        // POST: api/motos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MotoRequest request)
        {
            var moto = new Moto
            {
                Placa = request.Placa,
                Modelo = request.Modelo
            };
            await _motoRepo.AddAsync(moto);

            if (request.EchoBeaconId.HasValue)
            {
                var beacon = await _beaconRepo.FindByIdAsync(request.EchoBeaconId.Value);
                if (beacon == null)
                    return BadRequest($"EchoBeacon {request.EchoBeaconId} não encontrada.");

                beacon.MotoId = moto.Id;
                await _beaconRepo.UpdateAsync(beacon);
            }

            var m = await _motoRepo
                .QueryWithFilters(new MotoFilter())
                .Include(x => x.EchoBeacon)
                .FirstOrDefaultAsync(x => x.Id == moto.Id)!;

            var dto = new MotoResponse
            {
                Id = m.Id,
                Placa = m.Placa,
                Modelo = m.Modelo,
                EchoBeacon = m.EchoBeacon is null
                    ? null
                    : new EchoBeaconResponse
                    {
                        Id = m.EchoBeacon.Id,
                        NumeroIdentificacao = m.EchoBeacon.NumeroIdentificacao,
                        DataRegistro = m.EchoBeacon.DataRegistro,
                        Moto = null
                    }
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        // PUT: api/motos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] MotoRequest request)
        {
            var moto = await _motoRepo.FindByIdAsync(id);
            if (moto == null)
                return NotFound();

            moto.Placa = request.Placa;
            moto.Modelo = request.Modelo;
            await _motoRepo.UpdateAsync(moto);

            if (request.EchoBeaconId.HasValue)
            {
                var newBeacon = await _beaconRepo.FindByIdAsync(request.EchoBeaconId.Value);
                if (newBeacon == null)
                    return BadRequest($"EchoBeacon {request.EchoBeaconId} não encontrada.");

                var old = await _beaconRepo
                    .QueryWithFilters(new EchoBeaconFilter())
                    .FirstOrDefaultAsync(e => e.MotoId == id);
                if (old != null && old.Id != newBeacon.Id)
                {
                    old.MotoId = null;
                    await _beaconRepo.UpdateAsync(old);
                }

                newBeacon.MotoId = id;
                await _beaconRepo.UpdateAsync(newBeacon);
            }
            else
            {
                var old = await _beaconRepo
                    .QueryWithFilters(new EchoBeaconFilter())
                    .FirstOrDefaultAsync(e => e.MotoId == id);
                if (old != null)
                {
                    old.MotoId = null;
                    await _beaconRepo.UpdateAsync(old);
                }
            }

            return NoContent();
        }

        // DELETE: api/motos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var removed = await _motoRepo.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
