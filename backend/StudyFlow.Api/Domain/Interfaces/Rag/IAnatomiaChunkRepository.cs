using Pgvector;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IAnatomiaChunkRepository
{
    Task<IReadOnlyList<AnatomiaChunkVector>> BuscarPublicadosPorSimilaridadeAsync(Vector embedding, int quantidade, CancellationToken cancellationToken = default);
}
