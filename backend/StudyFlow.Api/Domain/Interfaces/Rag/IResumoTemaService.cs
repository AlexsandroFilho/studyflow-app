using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IResumoTemaService
{
    Task<ResumoTemaResponseDto> CriarAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ResumoTemaResponseDto>> ListarAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default);
}
