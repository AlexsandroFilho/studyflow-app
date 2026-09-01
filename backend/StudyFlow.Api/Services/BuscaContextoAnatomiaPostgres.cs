using Pgvector;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;

namespace StudyFlow.Api.Services;

public sealed class BuscaContextoAnatomiaPostgres(IAnatomiaChunkRepository anatomiaChunkRepository, IEmbeddingService embeddingService) : IBuscaContextoAnatomia
{
    public async Task<IReadOnlyList<ContextoAnatomiaDto>> BuscarAsync(string consulta, int quantidade, CancellationToken cancellationToken = default)
    {
        var embedding = new Vector(await embeddingService.GerarAsync(consulta, TipoTarefaEmbedding.Consulta, cancellationToken));

        var chunks = await anatomiaChunkRepository.BuscarPublicadosPorSimilaridadeAsync(embedding, quantidade, cancellationToken);
        return chunks.Select(chunk => chunk.ToContextoDto(embedding)).ToList();
    }
}
