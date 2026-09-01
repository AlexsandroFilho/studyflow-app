using StudyFlow.Api.Domain.Enums;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IModeloIaClient
{
    Task<float[]> GerarEmbeddingAsync(string texto, TipoTarefaEmbedding tipoTarefa, CancellationToken cancellationToken = default);
    Task<string> GerarJsonAsync(string instrucaoSistema, string prompt, CancellationToken cancellationToken = default);
}
