using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Data.Repositories;

public sealed class ContextoGrafoNotasRepository(AppDbContext dbContext) : IContextoGrafoNotasRepository
{
    public Task<Nota?> ObterNotaDoUsuarioAsync(int notaId, Guid usuarioId, CancellationToken cancellationToken = default) =>
        dbContext.Notas.AsNoTracking().SingleOrDefaultAsync(x => x.Id == notaId && x.UsuarioId == usuarioId, cancellationToken);

    public async Task<IReadOnlyList<ConexaoNota>> ListarConexoesComNotasAsync(int notaId, CancellationToken cancellationToken = default) =>
        await dbContext.ConexaoNotas.AsNoTracking()
            .Where(x => x.NotaOrigemId == notaId || x.NotaDestinoId == notaId)
            .Include(x => x.NotaOrigem)
            .Include(x => x.NotaDestino)
            .ToListAsync(cancellationToken);
}
