using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Data.Repositories;

public sealed class IngestaoFonteAnatomiaRepository(AppDbContext dbContext) : IIngestaoFonteAnatomiaRepository
{
    public Task AdicionarAsync(IngestaoFonteAnatomia ingestao, CancellationToken cancellationToken = default) =>
        dbContext.AddAsync(ingestao, cancellationToken).AsTask();

    public Task<IngestaoFonteAnatomia?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Set<IngestaoFonteAnatomia>().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<IngestaoFonteAnatomia?> ObterProximaPendenteAsync(CancellationToken cancellationToken = default) =>
        dbContext.Set<IngestaoFonteAnatomia>()
            .Where(x => x.Status == StatusIngestaoFonteAnatomia.Pendente)
            .OrderBy(x => x.DataCriacao)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<IngestaoFonteAnatomia>> ListarAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Set<IngestaoFonteAnatomia>()
            .AsNoTracking()
            .OrderByDescending(x => x.DataCriacao)
            .Take(50)
            .ToListAsync(cancellationToken);

    public async Task ReenfileirarProcessamentosInterrompidosAsync(CancellationToken cancellationToken = default)
    {
        var interrompidos = await dbContext.Set<IngestaoFonteAnatomia>()
            .Where(x => x.Status == StatusIngestaoFonteAnatomia.Processando)
            .ToListAsync(cancellationToken);

        foreach (var ingestao in interrompidos)
            ingestao.Reenfileirar();

        if (interrompidos.Count > 0)
            await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
