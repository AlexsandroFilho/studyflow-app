using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using StudyFlow.Api.Data;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Notas;
using StudyFlow.Api.Domain.Interfaces.Temas;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Services;
using Xunit;

namespace StudyFlow.Api.Tests;

public class NotaServiceTests
{
    private readonly Mock<INotaRepository> notaRepository = new();
    private readonly Mock<ITemaRepository> temaRepository = new();
    private readonly Faker faker = new();

    [Fact]
    public async Task ListarTodasAsync_QuandoExistemNotas_DeveRetornarNotasMapeadas()
    {
        var tema = CriarTema();
        var notas = new[] { CriarNota(tema), CriarNota(tema) };
        notaRepository.Setup(repository => repository.ListarTodasAsync()).ReturnsAsync(notas);
        var service = CriarService();

        var resultado = await service.ListarTodasAsync();

        resultado.Should().HaveCount(2);
        resultado.Should().OnlyContain(nota => nota.TemaId == tema.Id && nota.NomeTema == tema.Nome);
    }

    [Fact]
    public async Task ObterPorTemaIdAsync_QuandoExistemNotasDoTema_DeveRetornarNotasMapeadas()
    {
        var tema = CriarTema();
        var notas = new[] { CriarNota(tema) };
        notaRepository.Setup(repository => repository.ObterPorTemaIdAsync(tema.Id)).ReturnsAsync(notas);
        var service = CriarService();

        var resultado = await service.ObterPorTemaIdAsync(tema.Id);

        resultado.Should().ContainSingle();
        resultado.Single().Id.Should().Be(notas[0].Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoNotaExiste_DeveRetornarNotaMapeada()
    {
        var nota = CriarNota(CriarTema());
        notaRepository.Setup(repository => repository.ObterPorIdAsync(nota.Id)).ReturnsAsync(nota);
        var service = CriarService();

        var resultado = await service.ObterPorIdAsync(nota.Id);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(nota.Id);
        resultado.Titulo.Should().Be(nota.Titulo);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoNotaNaoExiste_DeveRetornarNulo()
    {
        notaRepository.Setup(repository => repository.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Nota?)null);
        var service = CriarService();

        var resultado = await service.ObterPorIdAsync(999);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_QuandoTemaExiste_DeveCriarNotaSalvarEretornarNota()
    {
        var tema = CriarTema();
        var dto = new CreateNotaDto(faker.Lorem.Sentence(), faker.Lorem.Paragraph(), tema.Id, faker.Lorem.Sentence());
        Nota? notaCriada = null;
        temaRepository.Setup(repository => repository.ObterPorIdAsync(tema.Id)).ReturnsAsync(tema);
        notaRepository.Setup(repository => repository.CriarAsync(It.IsAny<Nota>()))
            .Callback<Nota>(nota => notaCriada = nota)
            .Returns(Task.CompletedTask);
        notaRepository.Setup(repository => repository.SalvarAlteracoesAsync()).ReturnsAsync(true);
        var service = CriarService();

        var resultado = await service.CriarAsync(dto);

        resultado.Should().NotBeNull();
        resultado!.Titulo.Should().Be(dto.Titulo);
        resultado.Conteudo.Should().Be(dto.Conteudo);
        resultado.ResumoIa.Should().Be(dto.ResumoIa);
        resultado.TemaId.Should().Be(tema.Id);
        resultado.NomeTema.Should().Be(tema.Nome);
        notaCriada.Should().NotBeNull();
        notaRepository.Verify(repository => repository.CriarAsync(It.IsAny<Nota>()), Times.Once);
        notaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_QuandoTemaNaoExiste_NaoDeveCriarNotaNemSalvarERetornarNulo()
    {
        var dto = new CreateNotaDto(faker.Lorem.Sentence(), faker.Lorem.Paragraph(), faker.Random.Int(1, 100));
        temaRepository.Setup(repository => repository.ObterPorIdAsync(dto.TemaId!.Value)).ReturnsAsync((Tema?)null);
        var service = CriarService();

        var resultado = await service.CriarAsync(dto);

        resultado.Should().BeNull();
        notaRepository.Verify(repository => repository.CriarAsync(It.IsAny<Nota>()), Times.Never);
        notaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoNotaETemaExistem_DeveAtualizarCamposESalvar()
    {
        var temaAtual = CriarTema();
        var novoTema = CriarTema();
        var nota = CriarNota(temaAtual);
        var dto = new UpdateNotaDto(faker.Lorem.Sentence(), faker.Lorem.Paragraph(), novoTema.Id, faker.Lorem.Sentence());
        notaRepository.Setup(repository => repository.ObterPorIdAsync(nota.Id)).ReturnsAsync(nota);
        temaRepository.Setup(repository => repository.ObterPorIdAsync(novoTema.Id)).ReturnsAsync(novoTema);
        notaRepository.Setup(repository => repository.SalvarAlteracoesAsync()).ReturnsAsync(true);
        var service = CriarService();

        var resultado = await service.AtualizarAsync(nota.Id, dto);

        resultado.Should().BeTrue();
        nota.Titulo.Should().Be(dto.Titulo);
        nota.Conteudo.Should().Be(dto.Conteudo);
        nota.TemaId.Should().Be(novoTema.Id);
        nota.Tema.Should().Be(novoTema);
        nota.ResumoIA.Should().Be(dto.ResumoIa);
        notaRepository.Verify(repository => repository.Atualizar(nota), Times.Once);
        notaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoNotaNaoExiste_NaoDeveAtualizarNemSalvarERetornarFalso()
    {
        var dto = new UpdateNotaDto(faker.Lorem.Sentence(), faker.Lorem.Paragraph(), 1);
        notaRepository.Setup(repository => repository.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Nota?)null);
        var service = CriarService();

        var resultado = await service.AtualizarAsync(1, dto);

        resultado.Should().BeFalse();
        notaRepository.Verify(repository => repository.Atualizar(It.IsAny<Nota>()), Times.Never);
        notaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoTemaNaoExiste_NaoDeveAtualizarNemSalvarERetornarFalso()
    {
        var nota = CriarNota(CriarTema());
        var dto = new UpdateNotaDto(faker.Lorem.Sentence(), faker.Lorem.Paragraph(), 999);
        notaRepository.Setup(repository => repository.ObterPorIdAsync(nota.Id)).ReturnsAsync(nota);
        temaRepository.Setup(repository => repository.ObterPorIdAsync(dto.TemaId!.Value)).ReturnsAsync((Tema?)null);
        var service = CriarService();

        var resultado = await service.AtualizarAsync(nota.Id, dto);

        resultado.Should().BeFalse();
        notaRepository.Verify(repository => repository.Atualizar(It.IsAny<Nota>()), Times.Never);
        notaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoResumoNaoFoiInformado_DevePreservarResumoExistente()
    {
        var nota = CriarNota(CriarTema());
        var resumoOriginal = nota.ResumoIA;
        var dto = new UpdateNotaDto(faker.Lorem.Sentence(), faker.Lorem.Paragraph(), nota.TemaId);
        notaRepository.Setup(repository => repository.ObterPorIdAsync(nota.Id)).ReturnsAsync(nota);
        temaRepository.Setup(repository => repository.ObterPorIdAsync(nota.TemaId!.Value)).ReturnsAsync(nota.Tema);
        notaRepository.Setup(repository => repository.SalvarAlteracoesAsync()).ReturnsAsync(true);
        var service = CriarService();

        var resultado = await service.AtualizarAsync(nota.Id, dto);

        resultado.Should().BeTrue();
        nota.ResumoIA.Should().Be(resumoOriginal);
    }

    [Fact]
    public async Task DeletarAsync_QuandoNotaExiste_DeveDeletarESalvarERetornarResultadoDoSalvamento()
    {
        var nota = CriarNota(CriarTema());
        notaRepository.Setup(repository => repository.ObterPorIdAsync(nota.Id)).ReturnsAsync(nota);
        notaRepository.Setup(repository => repository.SalvarAlteracoesAsync()).ReturnsAsync(true);
        var service = CriarService();

        var resultado = await service.DeletarAsync(nota.Id);

        resultado.Should().BeTrue();
        notaRepository.Verify(repository => repository.Deletar(nota), Times.Once);
        notaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletarAsync_QuandoNotaNaoExiste_NaoDeveDeletarNemSalvarERetornarFalso()
    {
        notaRepository.Setup(repository => repository.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Nota?)null);
        var service = CriarService();

        var resultado = await service.DeletarAsync(999);

        resultado.Should().BeFalse();
        notaRepository.Verify(repository => repository.Deletar(It.IsAny<Nota>()), Times.Never);
        notaRepository.Verify(repository => repository.SalvarAlteracoesAsync(), Times.Never);
    }

    private NotaService CriarService() => new(notaRepository.Object, temaRepository.Object, CriarDbContext());

    private static AppDbContext CriarDbContext()
    {
        var dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().Options)
        {
            CurrentUsuarioId = Guid.NewGuid()
        };
        return dbContext;
    }

    private Tema CriarTema() => new()
    {
        Id = faker.Random.Int(1, 1000),
        Nome = faker.Commerce.Department(),
        Descricao = faker.Lorem.Sentence()
    };

    private Nota CriarNota(Tema tema) => new()
    {
        Id = faker.Random.Int(1, 1000),
        Titulo = faker.Lorem.Sentence(),
        Conteudo = faker.Lorem.Paragraph(),
        ResumoIA = faker.Lorem.Sentence(),
        TemaId = tema.Id,
        Tema = tema
    };
}
