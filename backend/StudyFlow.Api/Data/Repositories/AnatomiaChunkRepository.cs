using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Data.Repositories;

public sealed class AnatomiaChunkRepository(AppDbContext dbContext) : IAnatomiaChunkRepository
{
    public async Task<IReadOnlyList<AnatomiaChunkVector>> BuscarPublicadosPorSimilaridadeAsync(Vector embedding, int quantidade, CancellationToken cancellationToken = default) =>
        await dbContext.AnatomiaChunks.AsNoTracking()
            .Include(x => x.FonteAnatomia)
            .Where(x => x.FonteAnatomia!.Publicada)
            .OrderBy(x => x.Embedding.CosineDistance(embedding))
            .Take(quantidade)
            .ToListAsync(cancellationToken);
}
