using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Controllers
{
    [ApiController]
    [Route("api/localizacoes")]
    public class LocalizacoesController : ControllerBase
    {
        private readonly ILocalizacaoRepository _repo;
        private readonly IMotoRepository _motoRepo;
        private readonly IEchoBeaconRepository _beaconRepo;

        public LocalizacoesController(
            ILocalizacaoRepository repo,
            IMotoRepository motoRepo,
            IEchoBeaconRepository beaconRepo)
        {
            _repo = repo;
            _motoRepo = motoRepo;
            _beaconRepo = beaconRepo;
        }

        /// <summary>
        /// Lista localizações com paginação e filtros.
        /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LocalizacaoResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<LocalizacaoResponse>>> GetAll(
            [FromQuery] LocalizacaoFilter filter,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            page = page < 1 ? 1 : page;
            size = size < 1 ? 10 : size;

            var baseQuery = _repo.QueryWithFilters(filter);
            var total = await baseQuery.CountAsync();
            var items = await baseQuery
                .OrderByDescending(l => l.DataHoraRegistro)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var responses = items.Select(ToResponse).ToList();

            var result = new PagedResult<LocalizacaoResponse>
            {
                Items = responses,
                Page = page,
                Size = size,
                TotalItems = total,
                TotalPages = (int)Math.Ceiling(total / (double)size),
                Links = BuildCollectionLinks(page, size)
            };
            return Ok(result);
        }

        /// <summary>
        /// Obtém uma localização por Id.
        /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LocalizacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LocalizacaoResponse>> GetById(long id)
        {
            var entity = await _repo.FindByIdAsync(id);
            if (entity == null) return NotFound();
            var resp = ToResponse(entity);
            return Ok(resp);
        }

        /// <summary>
        /// Cria um registro de localização para uma moto.
        /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(LocalizacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] LocalizacaoRequest req)
        {
            var moto = await _motoRepo.FindByIdAsync(req.MotoId);
            if (moto == null) return BadRequest($"Moto {req.MotoId} não encontrada.");

            EchoBeacon? beacon = null;
            if (req.EchoBeaconId.HasValue)
            {
                beacon = await _beaconRepo.FindByIdAsync(req.EchoBeaconId.Value);
                if (beacon == null) return BadRequest($"EchoBeacon {req.EchoBeaconId} não encontrada.");
                // alocar beacon à moto
                beacon.MotoId = moto.Id;
                await _beaconRepo.UpdateAsync(beacon);
            }

            var entity = new Localizacao
            {
                MotoId = req.MotoId,
                EchoBeaconId = req.EchoBeaconId,
                Setor = string.IsNullOrWhiteSpace(req.Setor) ? "Patio" : req.Setor,
                Status = req.Status,
                DataHoraRegistro = DateTime.UtcNow
            };
            await _repo.AddAsync(entity);

            var created = await _repo.FindByIdAsync(entity.Id);
            var dto = ToResponse(created!);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// Atualiza uma localização.
        /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(long id, [FromBody] LocalizacaoRequest req)
        {
            var existing = await _repo.FindByIdAsync(id);
            if (existing == null) return NotFound();

            var moto = await _motoRepo.FindByIdAsync(req.MotoId);
            if (moto == null) return BadRequest($"Moto {req.MotoId} não encontrada.");

            EchoBeacon? beacon = null;
            if (req.EchoBeaconId.HasValue)
            {
                beacon = await _beaconRepo.FindByIdAsync(req.EchoBeaconId.Value);
                if (beacon == null) return BadRequest($"EchoBeacon {req.EchoBeaconId} não encontrada.");
            }

            // regras: se status finalizada, liberar beacon
            if (req.Status == LocalizacaoStatus.Finalizada)
            {
                var old = await _beaconRepo
                    .QueryWithFilters(new EchoBeaconFilter())
                    .FirstOrDefaultAsync(b => b.MotoId == existing.MotoId);
                if (old != null)
                {
                    old.MotoId = null;
                    await _beaconRepo.UpdateAsync(old);
                }
                existing.EchoBeaconId = null;
            }
            else
            {
                existing.EchoBeaconId = req.EchoBeaconId;
                if (beacon != null)
                {
                    beacon.MotoId = req.MotoId;
                    await _beaconRepo.UpdateAsync(beacon);
                }
            }

            existing.MotoId = req.MotoId;
            existing.Setor = string.IsNullOrWhiteSpace(req.Setor) ? existing.Setor : req.Setor;
            existing.Status = req.Status;
            await _repo.UpdateAsync(existing);
            return NoContent();
        }

        /// <summary>
        /// Remove uma localização.
        /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            var ok = await _repo.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        private static LocalizacaoResponse ToResponse(Localizacao l)
        {
            return new LocalizacaoResponse
            {
                Id = l.Id,
                Setor = l.Setor ?? string.Empty,
                Status = l.Status,
                DataHoraRegistro = l.DataHoraRegistro,
                Moto = l.Moto == null ? null : new MotoResponse
                {
                    Id = l.Moto.Id,
                    Placa = l.Moto.Placa ?? string.Empty,
                    Modelo = l.Moto.Modelo ?? string.Empty,
                    EchoBeacon = null
                },
                EchoBeacon = l.EchoBeacon == null ? null : new EchoBeaconResponse
                {
                    Id = l.EchoBeacon.Id,
                    NumeroIdentificacao = l.EchoBeacon.NumeroIdentificacao ?? string.Empty,
                    DataRegistro = l.EchoBeacon.DataRegistro,
                    Moto = null
                }
            };
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
