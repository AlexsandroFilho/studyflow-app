using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IContextoGrafoNotasService
{
    Task<ContextoNotaDto?> ObterAsync(int notaId, Guid usuarioId, CancellationToken cancellationToken = default);
}
