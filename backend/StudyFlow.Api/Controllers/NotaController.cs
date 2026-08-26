
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.Domain.Interfaces.Notas;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Controllers
{
    [ApiController]
    [Route("api/v1/notas")]
    public class NotaController : ControllerBase
    {
        private readonly INotaService _notaService;

        public NotaController(INotaService notaService)
        {
            _notaService = notaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotaResponseDto>>> ListarTodas()
        {
            var notas = await _notaService.ListarTodasAsync();
            return Ok(notas);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NotaResponseDto>> ObterPorId(int id)
        {
            var nota = await _notaService.ObterPorIdAsync(id);
            if (nota == null)
                return NotFound(new { mensagem = $"Nota com ID {id} não foi encontrada." });

            return Ok(nota);
        }

        [HttpGet("tema/{temaId:int}")]
        public async Task<ActionResult<IEnumerable<NotaResponseDto>>> ObterPorTemaId(int temaId)
        {
            var notas = await _notaService.ObterPorTemaIdAsync(temaId);
            return Ok(notas);
        }

        [HttpPost]
        public async Task<ActionResult<NotaResponseDto>> Criar([FromBody] CreateNotaDto dto)
        {
            var notaCriada = await _notaService.CriarAsync(dto);
            if (notaCriada == null)
                return BadRequest(new { mensagem = $"Não foi possível criar a nota. O TemaId {dto.TemaId} não existe." });

            return CreatedAtAction(nameof(ObterPorId), new { id = notaCriada.Id }, notaCriada);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateNotaDto dto)
        {
            var atualizado = await _notaService.AtualizarAsync(id, dto);
            if (!atualizado)
                return NotFound(new { mensagem = $"Nota com ID {id} não foi encontrada." });

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var deletado = await _notaService.DeletarAsync(id);
            if (!deletado)
                return NotFound(new { mensagem = $"Nota com ID {id} não foi encontrada." });

            return NoContent();
        }
    }
}