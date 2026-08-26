using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.Domain.Interfaces.Conexao;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Controllers
{
    [ApiController]
    [Route("api/v1/conexoes")]
    public class ConexaoNotaController : Controller
    {
        private readonly IConexaoNotaService _conexaoNotaService;

        public ConexaoNotaController(IConexaoNotaService conexaoNotaService)
        {
            _conexaoNotaService = conexaoNotaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConexaoResponseDto>>> ListarTodas([FromQuery] int? temaId)
        {
            var conexoes = await _conexaoNotaService.ListarTodasAsync(temaId);
            return Ok(conexoes);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ConexaoResponseDto>> ObterPorId(int id)
        {
            var conexao = await _conexaoNotaService.ObterPorIdAsync(id);
            if (conexao == null)
                return NotFound(new { mensagem = $"Conexão com ID {id} não foi encontrada." });

            return Ok(conexao);
        }

        [HttpGet("nota/{notaId:int}")]
        public async Task<ActionResult<IEnumerable<ConexaoResponseDto>>> ObterPorNotaId(int notaId)
        {
            var conexoes = await _conexaoNotaService.ObterPorNotaIdAsync(notaId);
            return Ok(conexoes);
        }

        [HttpPost]
        public async Task<ActionResult<ConexaoResponseDto>> CriarConexao([FromBody] CreateConexaoDto dto)
        {
            try
            {
                var conexao = await _conexaoNotaService.CriarConexaoAsync(dto);
                if (conexao == null)
                    return BadRequest(new { mensagem = "Não foi possível criar a conexão. Verifique se as notas de origem e destino existem." });

                return CreatedAtAction(nameof(ObterPorId), new { id = conexao.Id }, conexao);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("nota-conectada")]
        public async Task<ActionResult> CriarNotaConectada([FromBody] CreateNotaConectadaDto dto)
        {
            var resultado = await _conexaoNotaService.CriarNotaConectadaAsync(dto);
            if (resultado == null)
                return BadRequest(new { mensagem = "Não foi possível criar a nota conectada. Verifique se a nota de origem e o tema existem." });

            return Ok(new
            {
                nota = resultado.Value.Nota,
                conexao = resultado.Value.Conexao
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletarPorId(int id)
        {
            var deletado = await _conexaoNotaService.DeletarPorIdAsync(id);
            if (!deletado)
                return NotFound(new { mensagem = $"Conexão com ID {id} não foi encontrada." });

            return NoContent();
        }

        [HttpDelete("par")]
        public async Task<IActionResult> DeletarPorPar([FromQuery] int origemId, [FromQuery] int destinoId)
        {
            var deletado = await _conexaoNotaService.DeletarPorParAsync(origemId, destinoId);
            if (!deletado)
                return NotFound(new { mensagem = $"Conexão entre as notas {origemId} e {destinoId} não foi encontrada." });

            return NoContent();
        }
    }
}