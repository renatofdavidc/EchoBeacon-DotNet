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
    private readonly ILocalizacaoRepository _localizacaoRepo;

        public MotosController(
            ILogger<MotosController> logger,
            IMotoRepository motoRepo,
            IEchoBeaconRepository beaconRepo,
            ILocalizacaoRepository localizacaoRepo)
        {
            _logger = logger;
            _motoRepo = motoRepo;
            _beaconRepo = beaconRepo;
            _localizacaoRepo = localizacaoRepo;
        }

        /// <summary>
        /// Lista motos com paginação e filtros.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<MotoResponse>>> GetAll(
            [FromQuery] MotoFilter filter,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            page = page < 1 ? 1 : page;
            size = size < 1 ? 10 : size;

            var baseQuery = _motoRepo
                .QueryWithFilters(filter)
                .Include(m => m.EchoBeacon);

            var total = await baseQuery.CountAsync();
            var motos = await baseQuery
                .OrderBy(m => m.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var dtos = motos.Select(m => new MotoResponse
            {
                Id = m.Id,
                Placa = m.Placa ?? string.Empty,
                Modelo = m.Modelo ?? string.Empty,
                EchoBeacon = m.EchoBeacon is null
                    ? null
                    : new EchoBeaconResponse
                    {
                        Id = m.EchoBeacon.Id,
                        NumeroIdentificacao = m.EchoBeacon.NumeroIdentificacao ?? string.Empty,
                        DataRegistro = m.EchoBeacon.DataRegistro,
                        Moto = null
                    }
            }).ToList();

            var result = new PagedResult<MotoResponse>
            {
                Items = dtos,
                Page = page,
                Size = size,
                TotalItems = total,
                TotalPages = (int)Math.Ceiling(total / (double)size),
                Links = BuildCollectionLinks(page, size)
            };

            return Ok(result);
        }

        /// <summary>
        /// Obtém uma moto pelo Id.
        /// </summary>
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
                Placa = m.Placa ?? string.Empty,
                Modelo = m.Modelo ?? string.Empty,
                EchoBeacon = m.EchoBeacon is null
                    ? null
                    : new EchoBeaconResponse
                    {
                        Id = m.EchoBeacon.Id,
                        NumeroIdentificacao = m.EchoBeacon.NumeroIdentificacao ?? string.Empty,
                        DataRegistro = m.EchoBeacon.DataRegistro,
                        Moto = null
                    }
            };

            return Ok(dto);
        }

    /// <summary>
    /// Cria uma moto e opcionalmente aloca uma EchoBeacon.
    /// Exemplo de payload:
    /// {
    ///   "placa": "ABC1D23",
    ///   "modelo": "Honda CG 160",
    ///   "echoBeaconId": 10
    /// }
    /// </summary>
        [HttpPost]
    [ProducesResponseType(typeof(MotoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] MotoRequest request)
        {
            var moto = new Moto
            {
                Placa = request.Placa,
                Modelo = request.Modelo
            };
            await _motoRepo.AddAsync(moto);

            long? allocatedBeaconId = null;
            if (request.EchoBeaconId.HasValue)
            {
                var beacon = await _beaconRepo.FindByIdAsync(request.EchoBeaconId.Value);
                if (beacon == null)
                    return BadRequest($"EchoBeacon {request.EchoBeaconId} não encontrada.");
                if (beacon.MotoId.HasValue && beacon.MotoId.Value != moto.Id)
                    return Conflict($"EchoBeacon {request.EchoBeaconId} já está alocada à moto {beacon.MotoId}.");

                beacon.MotoId = moto.Id;
                await _beaconRepo.UpdateAsync(beacon);
                allocatedBeaconId = beacon.Id;
            }
            else
            {
                var freeBeacon = await _beaconRepo
                    .QueryWithFilters(new EchoBeaconFilter())
                    .Where(b => b.MotoId == null)
                    .OrderBy(b => b.Id)
                    .FirstOrDefaultAsync();
                if (freeBeacon != null)
                {
                    freeBeacon.MotoId = moto.Id;
                    await _beaconRepo.UpdateAsync(freeBeacon);
                    allocatedBeaconId = freeBeacon.Id;
                }
            }

            var m = await _motoRepo
                .QueryWithFilters(new MotoFilter())
                .Include(x => x.EchoBeacon)
                .FirstOrDefaultAsync(x => x.Id == moto.Id);

            if (m == null)
                return StatusCode(500, "Erro ao carregar a moto recém-criada.");

            var dto = new MotoResponse
            {
                Id = m.Id,
                Placa = m.Placa ?? string.Empty,
                Modelo = m.Modelo ?? string.Empty,
                EchoBeacon = m.EchoBeacon is null
                    ? null
                    : new EchoBeaconResponse
                    {
                        Id = m.EchoBeacon.Id,
                        NumeroIdentificacao = m.EchoBeacon.NumeroIdentificacao ?? string.Empty,
                        DataRegistro = m.EchoBeacon.DataRegistro,
                        Moto = null
                    }
            };

            var initialLoc = new Localizacao
            {
                MotoId = moto.Id,
                EchoBeaconId = allocatedBeaconId,
                Setor = "Patio",
                Status = LocalizacaoStatus.Patio,
                DataHoraRegistro = DateTime.UtcNow
            };
            await _localizacaoRepo.AddAsync(initialLoc);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

    /// <summary>
    /// Atualiza dados de uma moto e seu vínculo com EchoBeacon.
    /// Exemplo de payload:
    /// {
    ///   "placa": "ABC1D23",
    ///   "modelo": "Yamaha Fazer",
    ///   "echoBeaconId": 15
    /// }
    /// </summary>
        [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
                if (newBeacon.MotoId.HasValue && newBeacon.MotoId.Value != id)
                    return Conflict($"EchoBeacon {request.EchoBeaconId} já está alocada à moto {newBeacon.MotoId}.");

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

    /// <summary>
    /// Remove uma moto.
    /// </summary>
        [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            // liberar beacon, se existir
            var old = await _beaconRepo
                .QueryWithFilters(new EchoBeaconFilter())
                .FirstOrDefaultAsync(e => e.MotoId == id);
            if (old != null)
            {
                old.MotoId = null;
                await _beaconRepo.UpdateAsync(old);
            }

            var removed = await _motoRepo.DeleteAsync(id);
            if (!removed)
                return NotFound();

            return NoContent();
        }

        private IEnumerable<LinkDto> BuildCollectionLinks(int page, int size)
        {
            var links = new List<LinkDto>
            {
                new LinkDto { Rel = "self", Href = Url.ActionLink(nameof(GetAll), values: new { page, size }) ?? string.Empty, Method = "GET" },
                new LinkDto { Rel = "create", Href = Url.ActionLink(nameof(Create)) ?? string.Empty, Method = "POST" }
            };
            if (page > 1)
                links.Add(new LinkDto { Rel = "prev", Href = Url.ActionLink(nameof(GetAll), values: new { page = page - 1, size }) ?? string.Empty, Method = "GET" });
            links.Add(new LinkDto { Rel = "next", Href = Url.ActionLink(nameof(GetAll), values: new { page = page + 1, size }) ?? string.Empty, Method = "GET" });
            return links;
        }
    }
}
