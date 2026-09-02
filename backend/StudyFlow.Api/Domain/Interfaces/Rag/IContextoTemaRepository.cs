using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IContextoTemaRepository
{
    Task<Tema?> ObterTemaDoUsuarioAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Nota>> ListarNotasDoTemaAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConexaoNota>> ListarConexoesInternasAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default);
}
