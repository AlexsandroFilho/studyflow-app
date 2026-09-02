using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;

namespace StudyFlow.Api.Services;

public sealed class ContextoTemaService(IContextoTemaRepository contextoRepository) : IContextoTemaService
{
    public async Task<ContextoTemaDto?> ObterAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var tema = await contextoRepository.ObterTemaDoUsuarioAsync(temaId, usuarioId, cancellationToken);
        if (tema is null)
            return null;

        var notas = await contextoRepository.ListarNotasDoTemaAsync(temaId, usuarioId, cancellationToken);
        var conexoes = await contextoRepository.ListarConexoesInternasAsync(temaId, usuarioId, cancellationToken);

        return tema.ToContextoTemaDto(
            notas.Select(x => x.ToContextoTemaDto()).ToList(),
            conexoes.Select(x => x.ToContextoTemaDto()).ToList());
    }
}
