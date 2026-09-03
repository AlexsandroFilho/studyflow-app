using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Usuarios;

public interface IUsuarioPreferenciasService
{
    Task<UsuarioPreferenciasResponse> AtualizarGuiaInicialAsync(
        Guid usuarioId,
        AtualizarPreferenciaGuiaRequest request,
        CancellationToken cancellationToken = default);
}
