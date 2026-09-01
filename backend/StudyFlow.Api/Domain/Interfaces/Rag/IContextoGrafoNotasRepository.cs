using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IContextoGrafoNotasRepository
{
    Task<Nota?> ObterNotaDoUsuarioAsync(int notaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConexaoNota>> ListarConexoesComNotasAsync(int notaId, CancellationToken cancellationToken = default);
}
