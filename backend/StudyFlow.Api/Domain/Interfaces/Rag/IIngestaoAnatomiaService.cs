using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IIngestaoAnatomiaService
{
    Task<FonteIngestaoResultado> IngerirAsync(FonteIngestaoRequest request, CancellationToken cancellationToken = default);
}
