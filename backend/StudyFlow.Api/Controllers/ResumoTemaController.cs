using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/temas/{temaId:int}/resumos")]
public class ResumoTemaController(IResumoTemaService resumoTemaService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ResumoTemaResponseDto>> Criar(int temaId, CancellationToken cancellationToken)
    {
        var resumo = await resumoTemaService.CriarAsync(temaId, ObterUsuarioId(), cancellationToken);
        return CreatedAtAction(nameof(Listar), new { temaId }, resumo);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ResumoTemaResponseDto>>> Listar(int temaId, CancellationToken cancellationToken) =>
        Ok(await resumoTemaService.ListarAsync(temaId, ObterUsuarioId(), cancellationToken));

    private Guid ObterUsuarioId()
    {
        var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(valor, out var usuarioId)
            ? usuarioId
            : throw new UnauthorizedAccessException("Usuário autenticado inválido.");
    }
}
