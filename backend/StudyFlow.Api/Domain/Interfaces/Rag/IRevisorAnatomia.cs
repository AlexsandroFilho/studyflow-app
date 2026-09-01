using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IRevisorAnatomia
{
    Task<ResultadoRevisaoNotaDto> RevisarAsync(ContextoNotaDto nota, IReadOnlyList<ContextoAnatomiaDto> evidencias, CancellationToken cancellationToken = default);
    string Modelo { get; }
}
