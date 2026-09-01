using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/notas/{notaId:int}/revisoes")]
public class RevisaoNotaController(IRevisaoNotaService revisaoNotaService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RevisaoNotaResponseDto>> Criar(int notaId, CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();
        var revisao = await revisaoNotaService.CriarAsync(notaId, usuarioId, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { notaId }, revisao);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RevisaoNotaResponseDto>>> Listar(int notaId, CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();
        return Ok(await revisaoNotaService.ListarAsync(notaId, usuarioId, cancellationToken));
    }

    private Guid ObterUsuarioId()
    {
        var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(valor, out var usuarioId)
            ? usuarioId
            : throw new UnauthorizedAccessException("Usuário autenticado inválido.");
    }
}
