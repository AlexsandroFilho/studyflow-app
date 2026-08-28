using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Temas;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/temas")]
    public class TemaController : ControllerBase
    {
        
        private readonly ITemaService _temaService;

        public TemaController(ITemaService temaService)
        {
            _temaService = temaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemaResponseDto>>> ListarTodos()
        {
            var temas = await _temaService.ListarTodosAsync();
            return Ok(temas);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TemaResponseDto>> ObterPorId(int id)
        {
            var tema = await _temaService.ObterPorIdAsync(id);

            if(tema == null)
                return NotFound(new {mensagem = $"Tema com Id {id} não foi encontrado"});

            return Ok(tema);
        }

        [HttpPost]
        public async Task<ActionResult<TemaResponseDto>> Criar([FromBody] CreateTemaDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var temaCriado = await _temaService.CriarAsync(dto);

            return CreatedAtAction(nameof(ObterPorId), new { id = temaCriado.Id}, temaCriado);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateTemaDto dto)
        {
            var atualizado = await _temaService.AtualizarAsync(id, dto);

            if(!atualizado)
                return NotFound(new { mensagem = $"Tema com Id {id} não foi encontrado"});

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var deletado = await _temaService.DeletarAsync(id);

            if(!deletado)
                return NotFound(new { mensagem = $"Tema com Id {id} não foi encontrado"});

            return NoContent();
        }
        
    }
}