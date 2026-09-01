using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Data.Repositories;

public sealed class RevisaoNotaRepository(AppDbContext dbContext) : IRevisaoNotaRepository
{
    public Task AdicionarAsync(RevisaoNota revisao, CancellationToken cancellationToken = default) =>
        dbContext.RevisoesNota.AddAsync(revisao, cancellationToken).AsTask();

    public async Task<IReadOnlyList<RevisaoNota>> ListarPorNotaEUsuarioAsync(int notaId, Guid usuarioId, CancellationToken cancellationToken = default) =>
        await dbContext.RevisoesNota.AsNoTracking()
            .Where(x => x.NotaId == notaId && x.UsuarioId == usuarioId)
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync(cancellationToken);

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
