using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Services;
using Xunit;

namespace StudyFlow.Api.Tests;

public class GeminiGeradorQuizTemaAnatomiaTests
{
    [Fact]
    public async Task GerarAsync_SemEvidencias_DeveRetornarEvidenciaInsuficienteSemChamarIa()
    {
        var modelo = new Mock<IModeloIaClient>();
        var resultado = await CriarService(modelo.Object).GerarAsync(CriarTema(), []);
        resultado.Status.Should().Be(StatusQuizTema.EvidenciaInsuficiente);
        resultado.Perguntas.Should().BeEmpty();
        modelo.Verify(x => x.GerarJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GerarAsync_ComCincoPerguntasValidas_DeveMapearReferenciasOficiais()
    {
        var modelo = new Mock<IModeloIaClient>();
        modelo.Setup(x => x.GerarJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(CriarJsonPerguntas(1));
        var evidencia = CriarEvidencia();
        var resultado = await CriarService(modelo.Object).GerarAsync(CriarTema(), [evidencia]);
        resultado.Status.Should().Be(StatusQuizTema.Gerado);
        resultado.Perguntas.Should().HaveCount(5).And.OnlyContain(x => x.Alternativas.Count == 4);
        resultado.Perguntas.Should().OnlyContain(x => x.Referencias.Count == 1 && x.Referencias[0].FonteId == evidencia.FonteId);
    }

    [Fact]
    public async Task GerarAsync_ComReferenciaInexistente_DeveDescartarQuizInteiro()
    {
        var modelo = new Mock<IModeloIaClient>();
        modelo.Setup(x => x.GerarJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(CriarJsonPerguntas(99));
        var resultado = await CriarService(modelo.Object).GerarAsync(CriarTema(), [CriarEvidencia()]);
        resultado.Status.Should().Be(StatusQuizTema.EvidenciaInsuficiente);
        resultado.Perguntas.Should().BeEmpty();
    }

    private static GeminiGeradorQuizTemaAnatomia CriarService(IModeloIaClient modelo) => new(modelo,
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Ai:ChatModel"] = "gemini-teste" }).Build());
    private static ContextoTemaDto CriarTema() => new(1, "Sistema muscular", null, [new(1, "Bíceps", "Conteúdo")], [new(1, 1, "Bíceps", 2, "Braquial", "flexão")]);
    private static ContextoAnatomiaDto CriarEvidencia() => new(Guid.NewGuid(), Guid.NewGuid(), "Fonte", 12, "Músculos", "Anatomia", "Evidência", 0.9);
    private static string CriarJsonPerguntas(int evidenciaId) => $$"""
        {"perguntas":[
          {"enunciado":"Pergunta 1?","alternativas":["A1","B1","C1","D1"],"indiceRespostaCorreta":0,"explicacao":"Explicação 1","evidenciaIds":[{{evidenciaId}}]},
          {"enunciado":"Pergunta 2?","alternativas":["A2","B2","C2","D2"],"indiceRespostaCorreta":1,"explicacao":"Explicação 2","evidenciaIds":[{{evidenciaId}}]},
          {"enunciado":"Pergunta 3?","alternativas":["A3","B3","C3","D3"],"indiceRespostaCorreta":2,"explicacao":"Explicação 3","evidenciaIds":[{{evidenciaId}}]},
          {"enunciado":"Pergunta 4?","alternativas":["A4","B4","C4","D4"],"indiceRespostaCorreta":3,"explicacao":"Explicação 4","evidenciaIds":[{{evidenciaId}}]},
          {"enunciado":"Pergunta 5?","alternativas":["A5","B5","C5","D5"],"indiceRespostaCorreta":0,"explicacao":"Explicação 5","evidenciaIds":[{{evidenciaId}}]}
        ]}
        """;
}
