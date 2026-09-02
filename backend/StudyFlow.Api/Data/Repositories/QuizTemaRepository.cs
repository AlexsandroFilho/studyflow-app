using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Data.Repositories;

public sealed class QuizTemaRepository(AppDbContext dbContext) : IQuizTemaRepository
{
    public Task AdicionarQuizAsync(QuizTema quiz, CancellationToken cancellationToken = default) =>
        dbContext.QuizzesTema.AddAsync(quiz, cancellationToken).AsTask();

    public Task<QuizTema?> ObterQuizDoUsuarioAsync(Guid quizId, Guid usuarioId, CancellationToken cancellationToken = default) =>
        dbContext.QuizzesTema
            .Include(x => x.Perguntas.OrderBy(pergunta => pergunta.Ordem))
            .SingleOrDefaultAsync(x => x.Id == quizId && x.UsuarioId == usuarioId, cancellationToken);

    public async Task<IReadOnlyList<QuizTema>> ListarQuizzesAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default) =>
        await dbContext.QuizzesTema.AsNoTracking()
            .Include(x => x.Perguntas.OrderBy(pergunta => pergunta.Ordem))
            .Where(x => x.TemaId == temaId && x.UsuarioId == usuarioId)
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync(cancellationToken);

    public Task AdicionarTentativaAsync(TentativaQuizTema tentativa, CancellationToken cancellationToken = default) =>
        dbContext.TentativasQuizTema.AddAsync(tentativa, cancellationToken).AsTask();

    public async Task<IReadOnlyList<TentativaQuizTema>> ListarTentativasAsync(Guid quizId, Guid usuarioId, CancellationToken cancellationToken = default) =>
        await dbContext.TentativasQuizTema.AsNoTracking()
            .Include(x => x.Respostas)
                .ThenInclude(x => x.Pergunta)
            .Where(x => x.QuizTemaId == quizId && x.UsuarioId == usuarioId)
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync(cancellationToken);

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
