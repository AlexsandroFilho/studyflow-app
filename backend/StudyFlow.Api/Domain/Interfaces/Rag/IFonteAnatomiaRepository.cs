using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IFonteAnatomiaRepository
{
    Task<FonteAnatomia?> ObterPorHashComChunksAsync(string hashConteudo, CancellationToken cancellationToken = default);
    Task AdicionarAsync(FonteAnatomia fonte, CancellationToken cancellationToken = default);
    Task AdicionarChunksAsync(IEnumerable<AnatomiaChunkVector> chunks, CancellationToken cancellationToken = default);
    void RemoverChunks(IEnumerable<AnatomiaChunkVector> chunks);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
