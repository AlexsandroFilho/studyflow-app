using FluentAssertions;
using Moq;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Usuarios;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Services;
using Xunit;

namespace StudyFlow.Api.Tests;

public sealed class UsuarioPreferenciasServiceTests
{
    [Fact]
    public async Task AtualizarGuiaInicialAsync_DeveSalvarPreferenciaNaConta()
    {
        var usuario = new Usuario { MostrarGuiaInicial = true };
        var repository = new Mock<IUsuarioRepository>();
        repository.Setup(x => x.ObterPorIdAsync(usuario.Id)).ReturnsAsync(usuario);
        repository.Setup(x => x.SalvarAlteracoesAsync()).ReturnsAsync(true);
        var service = new UsuarioPreferenciasService(repository.Object);

        var response = await service.AtualizarGuiaInicialAsync(
            usuario.Id,
            new AtualizarPreferenciaGuiaRequest(false));

        response.MostrarGuiaInicial.Should().BeFalse();
        usuario.MostrarGuiaInicial.Should().BeFalse();
        repository.Verify(x => x.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarGuiaInicialAsync_DeveFalharQuandoUsuarioNaoExiste()
    {
        var repository = new Mock<IUsuarioRepository>();
        repository.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuario?)null);
        var service = new UsuarioPreferenciasService(repository.Object);

        var action = () => service.AtualizarGuiaInicialAsync(
            Guid.NewGuid(),
            new AtualizarPreferenciaGuiaRequest(false));

        await action.Should().ThrowAsync<KeyNotFoundException>();
        repository.Verify(x => x.SalvarAlteracoesAsync(), Times.Never);
    }
}
