using StudyFlow.Api.Domain.Enums;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IEmbeddingService
{
    Task<float[]> GerarAsync(string texto, TipoTarefaEmbedding tipoTarefa, CancellationToken cancellationToken = default);
}
