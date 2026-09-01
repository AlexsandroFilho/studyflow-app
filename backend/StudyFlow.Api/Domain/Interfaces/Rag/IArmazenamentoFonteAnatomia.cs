namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IArmazenamentoFonteAnatomia
{
    Task<string> ArmazenarAsync(string caminhoArquivo, string hashConteudo, CancellationToken cancellationToken = default);
}
