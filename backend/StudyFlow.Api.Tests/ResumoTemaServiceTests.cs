using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Services;
using Xunit;

namespace StudyFlow.Api.Tests;

public class ResumoTemaServiceTests
{
    private readonly Mock<IResumoTemaRepository> resumoRepository = new();
    private readonly Mock<IContextoTemaService> contextoTemaService = new();
    private readonly Mock<IBuscaContextoAnatomia> buscaContexto = new();
    private readonly Mock<IResumidorTemaAnatomia> resumidor = new();
    private readonly Guid usuarioId = Guid.NewGuid();

    [Fact]
    public async Task CriarAsync_DeveUsarTodasAsNotasDoTemaEPersistirHistorico()
    {
        var tema = CriarContextoTema();
        var resultado = new ResultadoResumoTemaDto(StatusResumoTema.Gerado, "Resumo", ["Ponto"], [], [CriarReferencia()]);
        contextoTemaService.Setup(x => x.ObterAsync(tema.TemaId, usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(tema);
        buscaContexto.Setup(x => x.BuscarAsync(It.IsAny<string>(), 10, It.IsAny<CancellationToken>())).ReturnsAsync([CriarEvidencia()]);
        resumidor.SetupGet(x => x.Modelo).Returns("gemini-teste");
        resumidor.Setup(x => x.ResumirAsync(tema, It.IsAny<IReadOnlyList<ContextoAnatomiaDto>>(), It.IsAny<CancellationToken>())).ReturnsAsync(resultado);
        ResumoTema? salvo = null;
        resumoRepository.Setup(x => x.AdicionarAsync(It.IsAny<ResumoTema>(), It.IsAny<CancellationToken>()))
            .Callback<ResumoTema, CancellationToken>((item, _) => salvo = item).Returns(Task.CompletedTask);
        resumoRepository.Setup(x => x.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var resposta = await CriarService().CriarAsync(tema.TemaId, usuarioId);

        resposta.TemaId.Should().Be(tema.TemaId);
        salvo.Should().NotBeNull();
        salvo!.UsuarioId.Should().Be(usuarioId);
        buscaContexto.Verify(x => x.BuscarAsync(It.Is<string>(consulta => tema.Notas.All(nota => consulta.Contains(nota.Conteudo))), 10, It.IsAny<CancellationToken>()), Times.Once);
        resumoRepository.Verify(x => x.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_QuandoTemaNaoPertenceAoUsuario_NaoDeveConsultarNemPersistir()
    {
        contextoTemaService.Setup(x => x.ObterAsync(99, usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync((ContextoTemaDto?)null);

        var acao = () => CriarService().CriarAsync(99, usuarioId);

        await acao.Should().ThrowAsync<KeyNotFoundException>();
        buscaContexto.Verify(x => x.BuscarAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        resumoRepository.Verify(x => x.AdicionarAsync(It.IsAny<ResumoTema>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private ResumoTemaService CriarService() => new(resumoRepository.Object, contextoTemaService.Object, buscaContexto.Object, resumidor.Object,
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Ai:ContextoQuantidadeChunksTema"] = "10" }).Build());

    private static ContextoTemaDto CriarContextoTema() => new(1, "Músculos do braço", null,
        [new(1, "Bíceps", "Conteúdo do bíceps"), new(2, "Braquial", "Conteúdo do braquial"), new(3, "Tríceps", "Conteúdo do tríceps")],
        [new(11, 1, "Bíceps", 2, "Braquial", "flexão")]);

    private static ContextoAnatomiaDto CriarEvidencia() => new(Guid.NewGuid(), Guid.NewGuid(), "Fonte", 10, null, "Anatomia", "Texto", 0.9);
    private static ReferenciaAnatomiaDto CriarReferencia() => new(Guid.NewGuid(), "Fonte", 10, null, "Anatomia");
}
