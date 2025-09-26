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

        /// <summary>
        /// Lista EchoBeacons com paginação e filtros.
        /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EchoBeaconResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<EchoBeaconResponse>>> GetAll(
            [FromQuery] EchoBeaconFilter filter,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            page = page < 1 ? 1 : page;
            size = size < 1 ? 10 : size;

            var baseQuery = _repository
                .QueryWithFilters(filter)
                .Include(e => e.Moto);

            var total = await baseQuery.CountAsync();
            var beacons = await baseQuery
                .OrderBy(b => b.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var dtos = beacons.Select(e => new EchoBeaconResponse
            {
                Id = e.Id,
                NumeroIdentificacao = e.NumeroIdentificacao ?? string.Empty,
                DataRegistro = e.DataRegistro,
                Moto = e.Moto is null
                       ? null
                       : new MotoResponse
                       {
                           Id = e.Moto.Id,
                           Placa = e.Moto.Placa ?? string.Empty,
                           Modelo = e.Moto.Modelo ?? string.Empty,
                           EchoBeacon = null
                       }
            }).ToList();

            var result = new PagedResult<EchoBeaconResponse>
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
        /// Obtém uma EchoBeacon pelo Id.
        /// </summary>
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

    /// <summary>
    /// Cria uma EchoBeacon.
    /// Exemplo de payload:
    /// {
    ///   "numeroIdentificacao": "BEACON-01",
    ///   "dataRegistro": "2025-09-25T12:00:00Z",
    ///   "motoId": null
    /// }
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EchoBeaconResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                .FirstOrDefaultAsync(x => x.Id == beacon.Id);

            if (e == null)
                return StatusCode(500, "Erro ao carregar a EchoBeacon recém-criada.");

            var dto = new EchoBeaconResponse
            {
                Id = e.Id,
                NumeroIdentificacao = e.NumeroIdentificacao ?? string.Empty,
                DataRegistro = e.DataRegistro,
                Moto = e.Moto is null
                       ? null
                       : new MotoResponse
                       {
                           Id = e.Moto.Id,
                           Placa = e.Moto.Placa ?? string.Empty,
                           Modelo = e.Moto.Modelo ?? string.Empty,
                           EchoBeacon = null
                       }
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// Atualiza uma EchoBeacon.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
    /// Remove uma EchoBeacon.
        /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            var removed = await _repository.DeleteAsync(id);
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
