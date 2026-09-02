using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IResumoTemaRepository
{
    Task AdicionarAsync(ResumoTema resumo, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ResumoTema>> ListarPorTemaEUsuarioAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
