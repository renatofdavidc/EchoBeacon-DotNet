using Microsoft.AspNetCore.Mvc;
using ProjetoChallengeMottu.DTOs;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Interfaces;

namespace ProjetoChallengeMottu.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auditorias")]
    [ApiVersion("1.0")]
    public class AuditoriasController : ControllerBase
    {
        private readonly IAuditoriaMotoRepository _repository;

        public AuditoriasController(IAuditoriaMotoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<AuditoriaMotoResponse>>> GetAll([FromQuery] AuditoriaMotoFilter filter)
        {
            var result = await _repository.GetAllAsync(filter);
            var items = result.Items.Select(a => new AuditoriaMotoResponse
            {
                IdAuditoria = a.IdAuditoria,
                Usuario = a.Usuario,
                Operacao = a.Operacao,
                DataHora = a.DataHora,
                PlacaOld = a.PlacaOld,
                PlacaNew = a.PlacaNew,
                ProblemaOld = a.ProblemaOld,
                ProblemaNew = a.ProblemaNew
            }).ToList();

            return Ok(new PagedResult<AuditoriaMotoResponse>
            {
                Items = items,
                Page = result.Page,
                Size = result.Size,
                TotalItems = result.TotalItems
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuditoriaMotoResponse>> GetById(int id)
        {
            var a = await _repository.GetByIdAsync(id);
            if (a == null) return NotFound();

            return Ok(new AuditoriaMotoResponse
            {
                IdAuditoria = a.IdAuditoria,
                Usuario = a.Usuario,
                Operacao = a.Operacao,
                DataHora = a.DataHora,
                PlacaOld = a.PlacaOld,
                PlacaNew = a.PlacaNew,
                ProblemaOld = a.ProblemaOld,
                ProblemaNew = a.ProblemaNew
            });
        }
    }
}
