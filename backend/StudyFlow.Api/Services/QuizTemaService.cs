using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;

namespace StudyFlow.Api.Services;

public sealed class QuizTemaService(
    IQuizTemaRepository quizRepository,
    IContextoTemaService contextoTemaService,
    IBuscaContextoAnatomia buscaContextoAnatomia,
    IGeradorQuizTemaAnatomia gerador,
    IConfiguration configuration) : IQuizTemaService
{
    public async Task<QuizTemaResponseDto> CriarAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var tema = await contextoTemaService.ObterAsync(temaId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Tema não encontrado.");
        if (tema.Notas.Count == 0)
            throw new InvalidOperationException("Crie ao menos uma nota no tema antes de gerar um quiz.");

        var quantidadeChunks = configuration.GetSection("Ai").GetValue<int?>("ContextoQuantidadeChunksQuiz")
            ?? configuration.GetSection("Ai").GetValue<int?>("ContextoQuantidadeChunksTema")
            ?? 10;
        var evidencias = await buscaContextoAnatomia.BuscarAsync(MontarConsulta(tema), quantidadeChunks, cancellationToken);
        var resultado = await gerador.GerarAsync(tema, evidencias, cancellationToken);
        var quiz = resultado.ToEntity(temaId, usuarioId, gerador.Modelo);
        await quizRepository.AdicionarQuizAsync(quiz, cancellationToken);
        await quizRepository.SalvarAlteracoesAsync(cancellationToken);
        return quiz.ToPublicResponseDto();
    }

    public async Task<IReadOnlyList<QuizTemaResponseDto>> ListarAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        if (await contextoTemaService.ObterAsync(temaId, usuarioId, cancellationToken) is null)
            throw new KeyNotFoundException("Tema não encontrado.");
        var quizzes = await quizRepository.ListarQuizzesAsync(temaId, usuarioId, cancellationToken);
        return quizzes.Select(x => x.ToPublicResponseDto()).ToList();
    }

    public async Task<QuizTemaResponseDto> ObterAsync(Guid quizId, Guid usuarioId, CancellationToken cancellationToken = default) =>
        (await ObterQuizAsync(quizId, usuarioId, cancellationToken)).ToPublicResponseDto();

    public async Task<TentativaQuizTemaResponseDto> CriarTentativaAsync(Guid quizId, Guid usuarioId, CriarTentativaQuizRequestDto request, CancellationToken cancellationToken = default)
    {
        var quiz = await ObterQuizAsync(quizId, usuarioId, cancellationToken);
        if (quiz.Status != StatusQuizTema.Gerado || quiz.Perguntas.Count != 5)
            throw new InvalidOperationException("Este quiz não possui perguntas válidas para responder.");

        ValidarRespostas(request, quiz.Perguntas);
        var respostasPorPergunta = request.Respostas.ToDictionary(x => x.PerguntaId);
        var tentativa = new TentativaQuizTema
        {
            QuizTemaId = quiz.Id,
            UsuarioId = usuarioId,
            QuantidadeQuestoes = quiz.Perguntas.Count
        };
        tentativa.Respostas = quiz.Perguntas.Select(pergunta =>
        {
            var indice = respostasPorPergunta[pergunta.Id].IndiceAlternativa;
            var acertou = indice == pergunta.IndiceRespostaCorreta;
            return new RespostaTentativaQuizTema
            {
                TentativaQuizTemaId = tentativa.Id,
                QuizTemaPerguntaId = pergunta.Id,
                Pergunta = pergunta,
                IndiceAlternativaSelecionada = indice,
                Acertou = acertou
            };
        }).ToList();
        tentativa.QuantidadeAcertos = tentativa.Respostas.Count(x => x.Acertou);

        await quizRepository.AdicionarTentativaAsync(tentativa, cancellationToken);
        await quizRepository.SalvarAlteracoesAsync(cancellationToken);
        return tentativa.ToResponseDto();
    }

    public async Task<IReadOnlyList<TentativaQuizTemaResponseDto>> ListarTentativasAsync(Guid quizId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        await ObterQuizAsync(quizId, usuarioId, cancellationToken);
        var tentativas = await quizRepository.ListarTentativasAsync(quizId, usuarioId, cancellationToken);
        return tentativas.Select(x => x.ToResponseDto()).ToList();
    }

    private async Task<QuizTema> ObterQuizAsync(Guid quizId, Guid usuarioId, CancellationToken cancellationToken) =>
        await quizRepository.ObterQuizDoUsuarioAsync(quizId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Quiz não encontrado.");

    private static void ValidarRespostas(CriarTentativaQuizRequestDto request, ICollection<QuizTemaPergunta> perguntas)
    {
        if (request.Respostas is null || request.Respostas.Count != perguntas.Count || request.Respostas.Select(x => x.PerguntaId).Distinct().Count() != perguntas.Count)
            throw new InvalidOperationException("Responda todas as questões uma única vez antes de finalizar.");

        var perguntasPorId = perguntas.ToDictionary(x => x.Id);
        foreach (var resposta in request.Respostas)
        {
            if (!perguntasPorId.TryGetValue(resposta.PerguntaId, out var pergunta))
                throw new InvalidOperationException("Uma das respostas não pertence a este quiz.");
            var alternativas = QuizTemaMapper.DesserializarAlternativas(pergunta.AlternativasJson);
            if (resposta.IndiceAlternativa < 0 || resposta.IndiceAlternativa >= alternativas.Count)
                throw new InvalidOperationException("Uma das alternativas selecionadas é inválida.");
        }
    }

    private static string MontarConsulta(ContextoTemaDto tema)
    {
        var partes = new List<string> { $"Tema: {tema.Nome}" };
        if (!string.IsNullOrWhiteSpace(tema.Descricao)) partes.Add(tema.Descricao);
        partes.AddRange(tema.Notas.Select(x => $"Nota: {x.Titulo}\n{x.Conteudo}"));
        partes.AddRange(tema.Conexoes.Select(x => $"Relação: {x.TituloOrigem} -> {x.TituloDestino}; {x.Rotulo ?? "sem rótulo"}"));
        return string.Join("\n\n", partes);
    }
}
