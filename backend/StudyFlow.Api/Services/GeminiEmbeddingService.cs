using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Services;

public sealed class GeminiEmbeddingService(IModeloIaClient modeloIaClient) : IEmbeddingService
{
    public Task<float[]> GerarAsync(string texto, TipoTarefaEmbedding tipoTarefa, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("Não é possível gerar embedding de texto vazio.", nameof(texto));

        return modeloIaClient.GerarEmbeddingAsync(texto, tipoTarefa, cancellationToken);
    }
}
