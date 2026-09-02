using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IResumidorTemaAnatomia
{
    string Modelo { get; }
    Task<ResultadoResumoTemaDto> ResumirAsync(ContextoTemaDto tema, IReadOnlyList<ContextoAnatomiaDto> evidencias, CancellationToken cancellationToken = default);
}
