using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Rag;

public interface IQuizTemaRepository
{
    Task AdicionarQuizAsync(QuizTema quiz, CancellationToken cancellationToken = default);
    Task<QuizTema?> ObterQuizDoUsuarioAsync(Guid quizId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuizTema>> ListarQuizzesAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task AdicionarTentativaAsync(TentativaQuizTema tentativa, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TentativaQuizTema>> ListarTentativasAsync(Guid quizId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
