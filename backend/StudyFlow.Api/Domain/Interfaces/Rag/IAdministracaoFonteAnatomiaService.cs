using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IAdministracaoFonteAnatomiaService
{
    Task<IngestaoFonteAnatomiaResponseDto> SolicitarAsync(CriarIngestaoFonteAnatomiaRequest request, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IngestaoFonteAnatomiaResponseDto>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IngestaoFonteAnatomiaResponseDto> ReprocessarAsync(Guid ingestaoId, CancellationToken cancellationToken = default);
    Task<bool> ProcessarProximaAsync(CancellationToken cancellationToken = default);
    Task ReenfileirarInterrompidasAsync(CancellationToken cancellationToken = default);
}
