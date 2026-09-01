using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Data.Repositories;

public sealed class FonteAnatomiaRepository(AppDbContext dbContext) : IFonteAnatomiaRepository
{
    public Task<FonteAnatomia?> ObterPorHashComChunksAsync(string hashConteudo, CancellationToken cancellationToken = default) =>
        dbContext.FontesAnatomia.Include(x => x.Chunks)
            .SingleOrDefaultAsync(x => x.HashConteudo == hashConteudo, cancellationToken);

    public Task AdicionarAsync(FonteAnatomia fonte, CancellationToken cancellationToken = default) =>
        dbContext.FontesAnatomia.AddAsync(fonte, cancellationToken).AsTask();

    public Task AdicionarChunksAsync(IEnumerable<AnatomiaChunkVector> chunks, CancellationToken cancellationToken = default) =>
        dbContext.AnatomiaChunks.AddRangeAsync(chunks, cancellationToken);

    public void RemoverChunks(IEnumerable<AnatomiaChunkVector> chunks) => dbContext.AnatomiaChunks.RemoveRange(chunks);

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
