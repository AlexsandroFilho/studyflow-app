using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Services;
using Xunit;

namespace StudyFlow.Api.Tests;

public sealed class AdministracaoFonteAnatomiaServiceTests
{
    [Fact]
    public async Task SolicitarAsync_DevePersistirTrabalhoPendenteEAguardarProcessamento()
    {
        var repository = new Mock<IIngestaoFonteAnatomiaRepository>();
        var armazenamento = new Mock<IArmazenamentoFonteAnatomia>();
        armazenamento.Setup(x => x.ArmazenarTemporarioAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("ingestoes/teste.pdf");
        IngestaoFonteAnatomia? persistida = null;
        repository.Setup(x => x.AdicionarAsync(It.IsAny<IngestaoFonteAnatomia>(), It.IsAny<CancellationToken>()))
            .Callback<IngestaoFonteAnatomia, CancellationToken>((item, _) => persistida = item)
            .Returns(Task.CompletedTask);

        var service = CriarService(repository, armazenamento);
        await using var arquivo = new MemoryStream([1, 2, 3]);

        var resultado = await service.SolicitarAsync(new CriarIngestaoFonteAnatomiaRequest(
            arquivo, "anatomia.pdf", "Anatomia", null, "1ª edição", "Anatomia Humana", null), Guid.NewGuid());

        resultado.Status.Should().Be(StatusIngestaoFonteAnatomia.Pendente);
        persistida.Should().NotBeNull();
        persistida!.ArquivoTemporarioChave.Should().Be("ingestoes/teste.pdf");
        repository.Verify(x => x.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReprocessarAsync_DeveReenfileirarSomenteIngestaoComFalha()
    {
        var repository = new Mock<IIngestaoFonteAnatomiaRepository>();
        var armazenamento = new Mock<IArmazenamentoFonteAnatomia>();
        var ingestao = new IngestaoFonteAnatomia { Status = StatusIngestaoFonteAnatomia.Falhou, MensagemErro = "Limite da IA" };
        repository.Setup(x => x.ObterPorIdAsync(ingestao.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ingestao);
        var service = CriarService(repository, armazenamento);

        var resultado = await service.ReprocessarAsync(ingestao.Id);

        resultado.Status.Should().Be(StatusIngestaoFonteAnatomia.Pendente);
        resultado.MensagemErro.Should().BeNull();
    }

    private static AdministracaoFonteAnatomiaService CriarService(Mock<IIngestaoFonteAnatomiaRepository> repository, Mock<IArmazenamentoFonteAnatomia> armazenamento) => new(
        repository.Object,
        Mock.Of<IIngestaoAnatomiaService>(),
        armazenamento.Object,
        NullLogger<AdministracaoFonteAnatomiaService>.Instance);
}
