using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/v1/admin/fontes-anatomia/ingestoes")]
public sealed class AdminFonteAnatomiaController(IAdministracaoFonteAnatomiaService administracaoService) : ControllerBase
{
    [HttpPost]
    [RequestSizeLimit(25L * 1024 * 1024)]
    public async Task<ActionResult<IngestaoFonteAnatomiaResponseDto>> Criar([FromForm] CriarIngestaoFonteAnatomiaForm form, CancellationToken cancellationToken)
    {
        if (form.Arquivo is null || form.Arquivo.Length == 0)
            throw new InvalidOperationException("Selecione um PDF para enviar.");

        await using var arquivo = form.Arquivo.OpenReadStream();
        var resultado = await administracaoService.SolicitarAsync(new CriarIngestaoFonteAnatomiaRequest(
            arquivo,
            form.Arquivo.FileName,
            form.Titulo,
            form.Autor,
            form.Versao,
            form.Assunto,
            form.Subassunto), ObterUsuarioId(), cancellationToken);

        return AcceptedAtAction(nameof(Listar), new { id = resultado.Id }, resultado);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<IngestaoFonteAnatomiaResponseDto>>> Listar(CancellationToken cancellationToken) =>
        Ok(await administracaoService.ListarAsync(cancellationToken));

    [HttpPost("{ingestaoId:guid}/reprocessar")]
    public async Task<ActionResult<IngestaoFonteAnatomiaResponseDto>> Reprocessar(Guid ingestaoId, CancellationToken cancellationToken) =>
        Ok(await administracaoService.ReprocessarAsync(ingestaoId, cancellationToken));

    private Guid ObterUsuarioId()
    {
        var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(valor, out var usuarioId)
            ? usuarioId
            : throw new UnauthorizedAccessException("Usuário autenticado inválido.");
    }
}
