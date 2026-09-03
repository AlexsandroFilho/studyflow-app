using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IIngestaoFonteAnatomiaRepository
{
    Task AdicionarAsync(IngestaoFonteAnatomia ingestao, CancellationToken cancellationToken = default);
    Task<IngestaoFonteAnatomia?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IngestaoFonteAnatomia?> ObterProximaPendenteAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IngestaoFonteAnatomia>> ListarAsync(CancellationToken cancellationToken = default);
    Task ReenfileirarProcessamentosInterrompidosAsync(CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
