using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IRevisaoNotaService
{
    Task<RevisaoNotaResponseDto> CriarAsync(int notaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RevisaoNotaResponseDto>> ListarAsync(int notaId, Guid usuarioId, CancellationToken cancellationToken = default);
}
