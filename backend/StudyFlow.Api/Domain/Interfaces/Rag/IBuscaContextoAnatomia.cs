using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IBuscaContextoAnatomia
{
    Task<IReadOnlyList<ContextoAnatomiaDto>> BuscarAsync(string consulta, int quantidade, CancellationToken cancellationToken = default);
}
