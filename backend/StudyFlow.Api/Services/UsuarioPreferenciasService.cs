using StudyFlow.Api.Domain.Interfaces.Usuarios;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Services;

public sealed class UsuarioPreferenciasService(IUsuarioRepository usuarioRepository) : IUsuarioPreferenciasService
{
    public async Task<UsuarioPreferenciasResponse> AtualizarGuiaInicialAsync(
        Guid usuarioId,
        AtualizarPreferenciaGuiaRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.ObterPorIdAsync(usuarioId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        usuario.MostrarGuiaInicial = request.MostrarGuiaInicial;
        await usuarioRepository.SalvarAlteracoesAsync();

        return new UsuarioPreferenciasResponse(usuario.MostrarGuiaInicial);
    }
}
