using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;
using StudyFlow.Api.Services;
using Xunit;

namespace StudyFlow.Api.Tests;

public class QuizTemaServiceTests
{
    private readonly Mock<IQuizTemaRepository> repository = new();
    private readonly Mock<IContextoTemaService> contextoService = new();
    private readonly Mock<IBuscaContextoAnatomia> busca = new();
    private readonly Mock<IGeradorQuizTemaAnatomia> gerador = new();
    private readonly Guid usuarioId = Guid.NewGuid();

    [Fact]
    public async Task CriarAsync_DevePersistirQuizSemExporGabaritoNaResposta()
    {
        var contexto = new ContextoTemaDto(1, "Tema", null, [new(1, "Nota", "Conteúdo")], []);
        contextoService.Setup(x => x.ObterAsync(1, usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(contexto);
        busca.Setup(x => x.BuscarAsync(It.IsAny<string>(), 10, It.IsAny<CancellationToken>())).ReturnsAsync([CriarEvidencia()]);
        gerador.SetupGet(x => x.Modelo).Returns("gemini-teste");
        gerador.Setup(x => x.GerarAsync(contexto, It.IsAny<IReadOnlyList<ContextoAnatomiaDto>>(), It.IsAny<CancellationToken>())).ReturnsAsync(CriarResultadoGerado());
        repository.Setup(x => x.AdicionarQuizAsync(It.IsAny<QuizTema>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repository.Setup(x => x.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var resposta = await CriarService().CriarAsync(1, usuarioId);

        resposta.Perguntas.Should().HaveCount(5);
        JsonSerializer.Serialize(resposta).Should().NotContain("IndiceRespostaCorreta");
        repository.Verify(x => x.AdicionarQuizAsync(It.Is<QuizTema>(quiz => quiz.Perguntas.Count == 5 && quiz.UsuarioId == usuarioId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarTentativaAsync_DeveCorrigirEPersistirSemChamarIa()
    {
        var quiz = CriarQuiz();
        repository.Setup(x => x.ObterQuizDoUsuarioAsync(quiz.Id, usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(quiz);
        repository.Setup(x => x.AdicionarTentativaAsync(It.IsAny<TentativaQuizTema>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repository.Setup(x => x.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var request = new CriarTentativaQuizRequestDto(quiz.Perguntas.Select((pergunta, indice) => new RespostaPerguntaQuizRequestDto(pergunta.Id, indice == 0 ? pergunta.IndiceRespostaCorreta : 3)).ToList());

        var resposta = await CriarService().CriarTentativaAsync(quiz.Id, usuarioId, request);

        resposta.QuantidadeQuestoes.Should().Be(5);
        resposta.Correcoes.Should().HaveCount(5);
        repository.Verify(x => x.AdicionarTentativaAsync(It.IsAny<TentativaQuizTema>(), It.IsAny<CancellationToken>()), Times.Once);
        gerador.Verify(x => x.GerarAsync(It.IsAny<ContextoTemaDto>(), It.IsAny<IReadOnlyList<ContextoAnatomiaDto>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ObterAsync_QuizDeOutroUsuario_DeveRetornarNaoEncontrado()
    {
        repository.Setup(x => x.ObterQuizDoUsuarioAsync(It.IsAny<Guid>(), usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync((QuizTema?)null);
        var acao = () => CriarService().ObterAsync(Guid.NewGuid(), usuarioId);
        await acao.Should().ThrowAsync<KeyNotFoundException>();
    }

    private QuizTemaService CriarService() => new(repository.Object, contextoService.Object, busca.Object, gerador.Object,
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Ai:ContextoQuantidadeChunksQuiz"] = "10" }).Build());
    private static ResultadoGeracaoQuizTemaDto CriarResultadoGerado() => new(StatusQuizTema.Gerado, "Gerado",
        Enumerable.Range(1, 5).Select(x => new PerguntaQuizGeradaDto($"Pergunta {x}", ["A", "B", "C", "D"], x % 4, "Explicação", [CriarReferencia()])).ToList());
    private QuizTema CriarQuiz()
    {
        var quiz = CriarResultadoGerado().ToEntity(1, usuarioId, "gemini-teste");
        foreach (var pergunta in quiz.Perguntas) pergunta.QuizTema = quiz;
        return quiz;
    }
    private static ContextoAnatomiaDto CriarEvidencia() => new(Guid.NewGuid(), Guid.NewGuid(), "Fonte", 1, null, null, "Texto", 0.9);
    private static ReferenciaAnatomiaDto CriarReferencia() => new(Guid.NewGuid(), "Fonte", 1, null, null);
}
