namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IArmazenamentoFonteAnatomia
{
    Task<string> ArmazenarAsync(string caminhoArquivo, string hashConteudo, CancellationToken cancellationToken = default);
    Task<string> ArmazenarTemporarioAsync(Stream arquivo, string nomeArquivo, Guid ingestaoId, CancellationToken cancellationToken = default);
    Task<string> BaixarParaArquivoTemporarioAsync(string chave, CancellationToken cancellationToken = default);
    Task RemoverAsync(string chave, CancellationToken cancellationToken = default);
}
