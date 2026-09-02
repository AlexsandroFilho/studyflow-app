using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Controllers;

[ApiController]
[Authorize]
public class QuizTemaController(IQuizTemaService quizTemaService) : ControllerBase
{
    [HttpPost("api/v1/temas/{temaId:int}/quizzes")]
    public async Task<ActionResult<QuizTemaResponseDto>> Criar(int temaId, CancellationToken cancellationToken)
    {
        var quiz = await quizTemaService.CriarAsync(temaId, ObterUsuarioId(), cancellationToken);
        return CreatedAtAction(nameof(Obter), new { quizId = quiz.Id }, quiz);
    }

    [HttpGet("api/v1/temas/{temaId:int}/quizzes")]
    public async Task<ActionResult<IReadOnlyList<QuizTemaResponseDto>>> Listar(int temaId, CancellationToken cancellationToken) =>
        Ok(await quizTemaService.ListarAsync(temaId, ObterUsuarioId(), cancellationToken));

    [HttpGet("api/v1/quizzes/{quizId:guid}")]
    public async Task<ActionResult<QuizTemaResponseDto>> Obter(Guid quizId, CancellationToken cancellationToken) =>
        Ok(await quizTemaService.ObterAsync(quizId, ObterUsuarioId(), cancellationToken));

    [HttpPost("api/v1/quizzes/{quizId:guid}/tentativas")]
    public async Task<ActionResult<TentativaQuizTemaResponseDto>> CriarTentativa(Guid quizId, CriarTentativaQuizRequestDto request, CancellationToken cancellationToken)
    {
        var tentativa = await quizTemaService.CriarTentativaAsync(quizId, ObterUsuarioId(), request, cancellationToken);
        return CreatedAtAction(nameof(ListarTentativas), new { quizId }, tentativa);
    }

    [HttpGet("api/v1/quizzes/{quizId:guid}/tentativas")]
    public async Task<ActionResult<IReadOnlyList<TentativaQuizTemaResponseDto>>> ListarTentativas(Guid quizId, CancellationToken cancellationToken) =>
        Ok(await quizTemaService.ListarTentativasAsync(quizId, ObterUsuarioId(), cancellationToken));

    private Guid ObterUsuarioId()
    {
        var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(valor, out var usuarioId)
            ? usuarioId
            : throw new UnauthorizedAccessException("Usuário autenticado inválido.");
    }
}
