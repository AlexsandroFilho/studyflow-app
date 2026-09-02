using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IQuizTemaService
{
    Task<QuizTemaResponseDto> CriarAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuizTemaResponseDto>> ListarAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<QuizTemaResponseDto> ObterAsync(Guid quizId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<TentativaQuizTemaResponseDto> CriarTentativaAsync(Guid quizId, Guid usuarioId, CriarTentativaQuizRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TentativaQuizTemaResponseDto>> ListarTentativasAsync(Guid quizId, Guid usuarioId, CancellationToken cancellationToken = default);
}
