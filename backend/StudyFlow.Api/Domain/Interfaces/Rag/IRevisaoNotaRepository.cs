using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IRevisaoNotaRepository
{
    Task AdicionarAsync(RevisaoNota revisao, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RevisaoNota>> ListarPorNotaEUsuarioAsync(int notaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
