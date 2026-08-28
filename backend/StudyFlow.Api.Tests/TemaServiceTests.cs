using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using StudyFlow.Api.Data;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Temas;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Services;
using Xunit;

namespace StudyFlow.Api.Tests;

public class TemaServiceTests
{
    private readonly Mock<ITemaRepository> temaRepository = new();
    private readonly Faker faker = new();

    [Fact]
    public async Task ListarTodosAsync_QuandoExistemTemas_DeveRetornarTemasMapeados()
    {
        var temas = new[] { CriarTema(), CriarTema() };
        temaRepository.Setup(repository => repository.ListarTodosAsync()).ReturnsAsync(temas);
        var service = CriarService();

        var resultado = await service.ListarTodosAsync();

        resultado.Should().HaveCount(2);
        resultado.Select(tema => tema.Id).Should().BeEquivalentTo(temas.Select(tema => tema.Id));
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoTemaExiste_DeveRetornarTemaMapeado()
    {
        var tema = CriarTema();
        temaRepository.Setup(repository => repository.ObterPorIdAsync(tema.Id)).ReturnsAsync(tema);
        var service = CriarService();

        var resultado = await service.ObterPorIdAsync(tema.Id);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(tema.Id);
        resultado.Nome.Should().Be(tema.Nome);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoTemaNaoExiste_DeveRetornarNulo()
    {
        temaRepository.Setup(repository => repository.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tema?)null);
        var service = CriarService();

        var resultado = await service.ObterPorIdAsync(999);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_QuandoDadosForemValidos_DeveNormalizarCriarSalvarERetornarTema()
    {
        var dto = new CreateTemaDto($"  {faker.Commerce.Department()}  ", $"  {faker.Lorem.Sentence()}  ");
        Tema? temaCriado = null;
        temaRepository.Setup(repository => repository.CriarAsync(It.IsAny<Tema>()))
            .Callback<Tema>(tema => temaCriado = tema)
            .Returns(Task.CompletedTask);
        temaRepository.Setup(repository => repository.SalvarAlteracoesAsync()).ReturnsAsync(true);
        var service = CriarService();

        var resultado = await service.CriarAsync(dto);

        resultado.Nome.Should().Be(dto.Nome.Trim());
        resultado.Descricao.Should().Be(dto.Descricao!.Trim());
        temaCriado.Should().NotBeNull();
        temaRepository.Verify(repository => repository.CriarAsync(It.IsAny<Tema>()), Times.Once);
        temaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoTemaExiste_DeveNormalizarAtualizarESalvar()
    {
        var tema = CriarTema();
        var dto = new UpdateTemaDto($"  {faker.Commerce.Department()}  ", $"  {faker.Lorem.Sentence()}  ");
        temaRepository.Setup(repository => repository.ObterPorIdAsync(tema.Id)).ReturnsAsync(tema);
        temaRepository.Setup(repository => repository.SalvarAlteracoesAsync()).ReturnsAsync(true);
        var service = CriarService();

        var resultado = await service.AtualizarAsync(tema.Id, dto);

        resultado.Should().BeTrue();
        tema.Nome.Should().Be(dto.Nome.Trim());
        tema.Descricao.Should().Be(dto.Descricao!.Trim());
        temaRepository.Verify(repository => repository.Atualizar(tema), Times.Once);
        temaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoTemaNaoExiste_NaoDeveAtualizarNemSalvarERetornarFalso()
    {
        var dto = new UpdateTemaDto(faker.Commerce.Department(), faker.Lorem.Sentence());
        temaRepository.Setup(repository => repository.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tema?)null);
        var service = CriarService();

        var resultado = await service.AtualizarAsync(999, dto);

        resultado.Should().BeFalse();
        temaRepository.Verify(repository => repository.Atualizar(It.IsAny<Tema>()), Times.Never);
        temaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeletarAsync_QuandoTemaExiste_DeveDeletarESalvarERetornarResultadoDoSalvamento()
    {
        var tema = CriarTema();
        temaRepository.Setup(repository => repository.ObterPorIdAsync(tema.Id)).ReturnsAsync(tema);
        temaRepository.Setup(repository => repository.SalvarAlteracoesAsync()).ReturnsAsync(false);
        var service = CriarService();

        var resultado = await service.DeletarAsync(tema.Id);

        resultado.Should().BeFalse();
        temaRepository.Verify(repository => repository.Deletar(tema), Times.Once);
        temaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletarAsync_QuandoTemaNaoExiste_NaoDeveDeletarNemSalvarERetornarFalso()
    {
        temaRepository.Setup(repository => repository.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tema?)null);
        var service = CriarService();

        var resultado = await service.DeletarAsync(999);

        resultado.Should().BeFalse();
        temaRepository.Verify(repository => repository.Deletar(It.IsAny<Tema>()), Times.Never);
        temaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Never);
    }

    private TemaService CriarService() => new(temaRepository.Object, CriarDbContext());

    private static AppDbContext CriarDbContext()
    {
        return new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().Options)
        {
            CurrentUsuarioId = Guid.NewGuid()
        };
    }

    private Tema CriarTema() => new()
    {
        Id = faker.Random.Int(1, 1000),
        Nome = faker.Commerce.Department(),
        Descricao = faker.Lorem.Sentence()
    };
}
