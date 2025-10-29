using Microsoft.AspNetCore.Mvc;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Models;

namespace ProjetoChallengeMottu.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/echobeacons")]
    [ApiVersion("1.0")]
    public class EchoBeaconsController : ControllerBase
    {
        private readonly IEchoBeaconRepository _echoBeaconRepository;
        private readonly IFuncionarioRepository _funcionarioRepository;

        public EchoBeaconsController(
            IEchoBeaconRepository echoBeaconRepository,
            IFuncionarioRepository funcionarioRepository)
        {
            _echoBeaconRepository = echoBeaconRepository;
            _funcionarioRepository = funcionarioRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<EchoBeaconResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<EchoBeaconResponse>>> GetAll([FromQuery] EchoBeaconFilter filter)
        {
            var result = await _echoBeaconRepository.GetAllAsync(filter);
            var items = result.Items.Select(e => new EchoBeaconResponse
            {
                IdEchoBeacon = e.IdEchoBeacon,
                CodigoIdentificador = e.CodigoIdentificador,
                StatusDispositivo = e.StatusDispositivo,
                TipoSinal = e.TipoSinal,
                RegistradaPor = e.RegistradaPor,
                NomeFuncionario = e.Funcionario?.Nome
            }).ToList();

            return Ok(new PagedResult<EchoBeaconResponse>
            {
                Items = items,
                Page = result.Page,
                Size = result.Size,
                TotalItems = result.TotalItems
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EchoBeaconResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EchoBeaconResponse>> GetById(int id)
        {
            var e = await _echoBeaconRepository.GetByIdAsync(id);
            if (e == null) return NotFound();

            return Ok(new EchoBeaconResponse
            {
                IdEchoBeacon = e.IdEchoBeacon,
                CodigoIdentificador = e.CodigoIdentificador,
                StatusDispositivo = e.StatusDispositivo,
                TipoSinal = e.TipoSinal,
                RegistradaPor = e.RegistradaPor,
                NomeFuncionario = e.Funcionario?.Nome
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(EchoBeaconResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] EchoBeaconRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var funcionarioExists = await _funcionarioRepository.ExistsAsync(request.RegistradaPor);
            if (!funcionarioExists) return BadRequest("Funcionário não encontrado");

            var beacon = new EchoBeacon
            {
                IdEchoBeacon = request.IdEchoBeacon,
                CodigoIdentificador = request.CodigoIdentificador,
                StatusDispositivo = request.StatusDispositivo,
                TipoSinal = request.TipoSinal,
                RegistradaPor = request.RegistradaPor
            };
            var created = await _echoBeaconRepository.AddAsync(beacon);

            var dto = new EchoBeaconResponse
            {
                IdEchoBeacon = created.IdEchoBeacon,
                CodigoIdentificador = created.CodigoIdentificador,
                StatusDispositivo = created.StatusDispositivo,
                TipoSinal = created.TipoSinal,
                RegistradaPor = created.RegistradaPor
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.IdEchoBeacon }, dto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] EchoBeaconRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var toUpdate = new EchoBeacon
            {
                IdEchoBeacon = request.IdEchoBeacon,
                CodigoIdentificador = request.CodigoIdentificador,
                StatusDispositivo = request.StatusDispositivo,
                TipoSinal = request.TipoSinal,
                RegistradaPor = request.RegistradaPor
            };

            var updated = await _echoBeaconRepository.UpdateAsync(id, toUpdate);
            if (updated == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _echoBeaconRepository.DeleteAsync(id);
            if (!removed) return NotFound();
            return NoContent();
        }
    }
}
