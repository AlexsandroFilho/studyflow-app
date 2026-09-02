using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Data.Repositories;

public sealed class ResumoTemaRepository(AppDbContext dbContext) : IResumoTemaRepository
{
    public Task AdicionarAsync(ResumoTema resumo, CancellationToken cancellationToken = default) =>
        dbContext.ResumosTema.AddAsync(resumo, cancellationToken).AsTask();

    public async Task<IReadOnlyList<ResumoTema>> ListarPorTemaEUsuarioAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default) =>
        await dbContext.ResumosTema.AsNoTracking()
            .Where(x => x.TemaId == temaId && x.UsuarioId == usuarioId)
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync(cancellationToken);

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
