using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IGeradorQuizTemaAnatomia
{
    string Modelo { get; }
    Task<ResultadoGeracaoQuizTemaDto> GerarAsync(ContextoTemaDto tema, IReadOnlyList<ContextoAnatomiaDto> evidencias, CancellationToken cancellationToken = default);
}
