using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Data.Repositories;

public sealed class ContextoTemaRepository(AppDbContext dbContext) : IContextoTemaRepository
{
    public Task<Tema?> ObterTemaDoUsuarioAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default) =>
        dbContext.Temas.AsNoTracking().SingleOrDefaultAsync(x => x.Id == temaId && x.UsuarioId == usuarioId, cancellationToken);

    public async Task<IReadOnlyList<Nota>> ListarNotasDoTemaAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default) =>
        await dbContext.Notas.AsNoTracking()
            .Where(x => x.TemaId == temaId && x.UsuarioId == usuarioId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ConexaoNota>> ListarConexoesInternasAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default) =>
        await dbContext.ConexaoNotas.AsNoTracking()
            .Include(x => x.NotaOrigem)
            .Include(x => x.NotaDestino)
            .Where(x => x.NotaOrigem!.TemaId == temaId && x.NotaDestino!.TemaId == temaId
                && x.NotaOrigem!.UsuarioId == usuarioId && x.NotaDestino!.UsuarioId == usuarioId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
}
