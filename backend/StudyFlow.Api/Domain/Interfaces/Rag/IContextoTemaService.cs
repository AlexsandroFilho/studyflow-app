using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IContextoTemaService
{
    Task<ContextoTemaDto?> ObterAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default);
}
