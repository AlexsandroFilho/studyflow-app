using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Services;
using Xunit;

namespace StudyFlow.Api.Tests;

public class GeminiResumidorTemaAnatomiaTests
{
    [Fact]
    public async Task ResumirAsync_SemEvidencias_DeveRetornarEvidenciaInsuficienteSemChamarIa()
    {
        var modelo = new Mock<IModeloIaClient>();

        var resultado = await CriarService(modelo.Object).ResumirAsync(CriarTema(), []);

        resultado.Status.Should().Be(StatusResumoTema.EvidenciaInsuficiente);
        modelo.Verify(x => x.GerarJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ResumirAsync_ComReferenciaEConexaoValidas_DeveMapearSomenteIdsPermitidos()
    {
        var modelo = new Mock<IModeloIaClient>();
        modelo.Setup(x => x.GerarJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("""{"status":"gerado","resumo":"Resumo","pontosChave":["Ponto"],"relacoes":[{"conexaoId":7,"descricao":"Relação válida"},{"conexaoId":99,"descricao":"Ignorar"}],"referencias":[{"evidenciaId":1},{"evidenciaId":99}]}""");
        var evidencia = new ContextoAnatomiaDto(Guid.NewGuid(), Guid.NewGuid(), "Fonte", 3, "Seção", "Anatomia", "Texto", 0.9);

        var resultado = await CriarService(modelo.Object).ResumirAsync(CriarTema(), [evidencia]);

        resultado.Status.Should().Be(StatusResumoTema.Gerado);
        resultado.Referencias.Should().ContainSingle().Which.Pagina.Should().Be(3);
        resultado.Relacoes.Should().ContainSingle().Which.ConexaoId.Should().Be(7);
    }

    [Fact]
    public async Task ResumirAsync_SemReferenciaValida_DeveRetornarEvidenciaInsuficiente()
    {
        var modelo = new Mock<IModeloIaClient>();
        modelo.Setup(x => x.GerarJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("""{"status":"gerado","resumo":"Resumo","pontosChave":[],"relacoes":[],"referencias":[{"evidenciaId":99}]}""");

        var resultado = await CriarService(modelo.Object).ResumirAsync(CriarTema(), [new ContextoAnatomiaDto(Guid.NewGuid(), Guid.NewGuid(), "Fonte", 3, null, null, "Texto", 0.9)]);

        resultado.Status.Should().Be(StatusResumoTema.EvidenciaInsuficiente);
    }

    private static GeminiResumidorTemaAnatomia CriarService(IModeloIaClient modelo) => new(modelo,
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Ai:ChatModel"] = "gemini-teste" }).Build());

    private static ContextoTemaDto CriarTema() => new(1, "Tema", null, [new(1, "Nota", "Conteúdo")], [new(7, 1, "Nota", 2, "Outra nota", "relação")]);
}
