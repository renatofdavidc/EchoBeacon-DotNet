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
    [Route("api/echobeacons")]
    public class EchoBeaconsController : ControllerBase
    {
        private readonly ILogger<EchoBeaconsController> _logger;
        private readonly IEchoBeaconRepository _repository;
        private readonly IMotoRepository _motoRepo;

        public EchoBeaconsController(
            ILogger<EchoBeaconsController> logger,
            IEchoBeaconRepository repository,
            IMotoRepository motoRepo)
        {
            _logger = logger;
            _repository = repository;
            _motoRepo = motoRepo;
        }

        // GET: api/echobeacons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EchoBeaconResponse>>> GetAll(
            [FromQuery] EchoBeaconFilter filter,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var beacons = await _repository
                .QueryWithFilters(filter)
                .Include(e => e.Moto)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var dtos = beacons.Select(e => new EchoBeaconResponse
            {
                Id = e.Id,
                NumeroIdentificacao = e.NumeroIdentificacao,
                DataRegistro = e.DataRegistro,
                Moto = e.Moto is null
                       ? null
                       : new MotoResponse
                       {
                           Id = e.Moto.Id,
                           Placa = e.Moto.Placa,
                           Modelo = e.Moto.Modelo,
                           EchoBeacon = null
                       }
            });

            return Ok(dtos);
        }

        // GET: api/echobeacons/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EchoBeaconResponse>> GetById(long id)
        {
            var e = await _repository
                .QueryWithFilters(new EchoBeaconFilter())
                .Include(x => x.Moto)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (e == null)
                return NotFound();

            var dto = new EchoBeaconResponse
            {
                Id = e.Id,
                NumeroIdentificacao = e.NumeroIdentificacao,
                DataRegistro = e.DataRegistro,
                Moto = e.Moto is null
                       ? null
                       : new MotoResponse
                       {
                           Id = e.Moto.Id,
                           Placa = e.Moto.Placa,
                           Modelo = e.Moto.Modelo,
                           EchoBeacon = null
                       }
            };

            return Ok(dto);
        }

        // POST: api/echobeacons
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EchoBeaconRequest request)
        {
            var beacon = new EchoBeacon
            {
                NumeroIdentificacao = request.NumeroIdentificacao,
                DataRegistro = request.DataRegistro,
                MotoId = request.MotoId
            };
            await _repository.AddAsync(beacon);

            var e = await _repository
                .QueryWithFilters(new EchoBeaconFilter())
                .Include(x => x.Moto)
                .FirstOrDefaultAsync(x => x.Id == beacon.Id)!;

            var dto = new EchoBeaconResponse
            {
                Id = e.Id,
                NumeroIdentificacao = e.NumeroIdentificacao,
                DataRegistro = e.DataRegistro,
                Moto = e.Moto is null
                       ? null
                       : new MotoResponse
                       {
                           Id = e.Moto.Id,
                           Placa = e.Moto.Placa,
                           Modelo = e.Moto.Modelo,
                           EchoBeacon = null
                       }
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        // PUT: api/echobeacons/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            [FromBody] EchoBeaconRequest request)
        {
            var existing = await _repository.FindByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.NumeroIdentificacao = request.NumeroIdentificacao;
            existing.DataRegistro = request.DataRegistro;
            existing.MotoId = request.MotoId;
            await _repository.UpdateAsync(existing);

            return NoContent();
        }

        // DELETE: api/echobeacons/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var removed = await _repository.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
