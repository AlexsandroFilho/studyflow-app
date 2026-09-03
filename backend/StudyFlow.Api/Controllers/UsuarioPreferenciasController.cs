using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.Domain.Interfaces.Usuarios;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/usuarios/preferencias")]
public sealed class UsuarioPreferenciasController(IUsuarioPreferenciasService preferenciasService) : ControllerBase
{
    [HttpPatch("guia-inicial")]
    public async Task<ActionResult<UsuarioPreferenciasResponse>> AtualizarGuiaInicial(
        [FromBody] AtualizarPreferenciaGuiaRequest request,
        CancellationToken cancellationToken)
    {
        var usuarioIdTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(usuarioIdTexto, out var usuarioId)) return Unauthorized();

        return Ok(await preferenciasService.AtualizarGuiaInicialAsync(usuarioId, request, cancellationToken));
    }
}
