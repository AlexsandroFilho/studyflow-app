using Bogus;
using FluentAssertions;
using Moq;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Conexao;
using StudyFlow.Api.Domain.Interfaces.Notas;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Services;
using Xunit;

namespace StudyFlow.Api.Tests;

public class ConexaoNotaServiceTests
{
    private readonly Mock<IConexaoNotaRepository> conexaoRepository = new();
    private readonly Mock<INotaRepository> notaRepository = new();
    private readonly Faker faker = new();

    [Fact]
    public async Task ListarTodasAsync_QuandoTemaNaoFoiInformado_DeveConsultarTodasAsConexoes()
    {
        var conexoes = new[] { CriarConexao(), CriarConexao() };
        conexaoRepository.Setup(repository => repository.ObterTodasAsync()).ReturnsAsync(conexoes);
        var service = CriarService();

        var resultado = await service.ListarTodasAsync();

        resultado.Should().HaveCount(2);
        conexaoRepository.Verify(repository => repository.ObterTodasAsync(), Times.Once);
        conexaoRepository.Verify(repository => repository.ObterPorTemaIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ListarTodasAsync_QuandoTemaFoiInformado_DeveConsultarConexoesDoTema()
    {
        var temaId = faker.Random.Int(1, 1000);
        var conexoes = new[] { CriarConexao() };
        conexaoRepository.Setup(repository => repository.ObterPorTemaIdAsync(temaId)).ReturnsAsync(conexoes);
        var service = CriarService();

        var resultado = await service.ListarTodasAsync(temaId);

        resultado.Should().ContainSingle();
        conexaoRepository.Verify(repository => repository.ObterPorTemaIdAsync(temaId), Times.Once);
        conexaoRepository.Verify(repository => repository.ObterTodasAsync(), Times.Never);
    }

    [Fact]
    public async Task ObterPorNotaIdAsync_QuandoExistemConexoes_DeveRetornarConexoesMapeadas()
    {
        var notaId = faker.Random.Int(1, 1000);
        var conexoes = new[] { CriarConexao() };
        conexaoRepository.Setup(repository => repository.ObterPorNotaIdAsync(notaId)).ReturnsAsync(conexoes);
        var service = CriarService();

        var resultado = await service.ObterPorNotaIdAsync(notaId);

        resultado.Should().ContainSingle();
        resultado.Single().NotaOrigemId.Should().Be(conexoes[0].NotaOrigemId);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoConexaoNaoExiste_DeveRetornarNulo()
    {
        conexaoRepository.Setup(repository => repository.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((ConexaoNota?)null);
        var service = CriarService();

        var resultado = await service.ObterPorIdAsync(999);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarConexaoAsync_QuandoNotasExistemENaoHaConexao_DeveCriarSalvarERetornarConexao()
    {
        var origemId = faker.Random.Int(1, 1000);
        var destinoId = faker.Random.Int(1001, 2000);
        var rotulo = $"  {faker.Lorem.Word()}  ";
        var dto = new CreateConexaoDto(origemId, destinoId, rotulo);
        notaRepository.Setup(repository => repository.ObterPorIdAsync(origemId)).ReturnsAsync(new Nota { Id = origemId });
        notaRepository.Setup(repository => repository.ObterPorIdAsync(destinoId)).ReturnsAsync(new Nota { Id = destinoId });
        conexaoRepository.Setup(repository => repository.ObterPorParAsync(origemId, destinoId)).ReturnsAsync((ConexaoNota?)null);
        conexaoRepository.Setup(repository => repository.SalvarAlteracoesAsync()).ReturnsAsync(true);
        var service = CriarService();

        var resultado = await service.CriarConexaoAsync(dto);

        resultado.Should().NotBeNull();
        resultado!.NotaOrigemId.Should().Be(origemId);
        resultado.NotaDestinoId.Should().Be(destinoId);
        resultado.Rotulo.Should().Be(rotulo.Trim());
        conexaoRepository.Verify(repository => repository.AdicionarAsync(It.Is<ConexaoNota>(conexao =>
            conexao.NotaOrigemId == origemId && conexao.NotaDestinoId == destinoId && conexao.Rotulo == rotulo.Trim())), Times.Once);
        conexaoRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(1000)]
    public async Task CriarConexaoAsync_QuandoOrigemEDestinoForemIguais_DeveLancarExcecao(int notaId)
    {
        var dto = new CreateConexaoDto(notaId, notaId, faker.Lorem.Word());
        var service = CriarService();

        var acao = () => service.CriarConexaoAsync(dto);

        await acao.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Não é permitido conectar uma nota a ela mesma.");
        notaRepository.Verify(repository => repository.ObterPorIdAsync(It.IsAny<int>()), Times.Never);
        conexaoRepository.Verify(repository => repository.AdicionarAsync(It.IsAny<ConexaoNota>()), Times.Never);
    }

    [Fact]
    public async Task CriarConexaoAsync_QuandoNotaDeOrigemNaoExiste_NaoDeveCriarNemSalvarERetornarNulo()
    {
        var dto = new CreateConexaoDto(1, 2, faker.Lorem.Word());
        notaRepository.Setup(repository => repository.ObterPorIdAsync(dto.NotaOrigemId)).ReturnsAsync((Nota?)null);
        var service = CriarService();

        var resultado = await service.CriarConexaoAsync(dto);

        resultado.Should().BeNull();
        conexaoRepository.Verify(repository => repository.ObterPorParAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        conexaoRepository.Verify(repository => repository.AdicionarAsync(It.IsAny<ConexaoNota>()), Times.Never);
    }

    [Fact]
    public async Task CriarConexaoAsync_QuandoConexaoJaExiste_DeveRetornarConexaoExistenteSemCriar()
    {
        var origemId = 1;
        var destinoId = 2;
        var existente = CriarConexao(origemId, destinoId);
        notaRepository.Setup(repository => repository.ObterPorIdAsync(origemId)).ReturnsAsync(new Nota { Id = origemId });
        notaRepository.Setup(repository => repository.ObterPorIdAsync(destinoId)).ReturnsAsync(new Nota { Id = destinoId });
        conexaoRepository.Setup(repository => repository.ObterPorParAsync(origemId, destinoId)).ReturnsAsync(existente);
        var service = CriarService();

        var resultado = await service.CriarConexaoAsync(new CreateConexaoDto(origemId, destinoId));

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(existente.Id);
        conexaoRepository.Verify(repository => repository.AdicionarAsync(It.IsAny<ConexaoNota>()), Times.Never);
        conexaoRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeletarPorIdAsync_QuandoConexaoExiste_DeveRemoverESalvarERetornarResultado()
    {
        var conexao = CriarConexao();
        conexaoRepository.Setup(repository => repository.ObterPorIdAsync(conexao.Id)).ReturnsAsync(conexao);
        conexaoRepository.Setup(repository => repository.SalvarAlteracoesAsync()).ReturnsAsync(true);
        var service = CriarService();

        var resultado = await service.DeletarPorIdAsync(conexao.Id);

        resultado.Should().BeTrue();
        conexaoRepository.Verify(repository => repository.Remover(conexao), Times.Once);
        conexaoRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletarPorParAsync_QuandoConexaoNaoExiste_NaoDeveRemoverNemSalvarERetornarFalso()
    {
        conexaoRepository.Setup(repository => repository.ObterPorParAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((ConexaoNota?)null);
        var service = CriarService();

        var resultado = await service.DeletarPorParAsync(1, 2);

        resultado.Should().BeFalse();
        conexaoRepository.Verify(repository => repository.Remover(It.IsAny<ConexaoNota>()), Times.Never);
        conexaoRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Never);
    }

    private ConexaoNotaService CriarService() => new(conexaoRepository.Object, notaRepository.Object);

    private ConexaoNota CriarConexao(int? origemId = null, int? destinoId = null) => new()
    {
        Id = faker.Random.Int(1, 1000),
        NotaOrigemId = origemId ?? faker.Random.Int(1, 1000),
        NotaDestinoId = destinoId ?? faker.Random.Int(1001, 2000),
        Rotulo = faker.Lorem.Word()
    };
}
