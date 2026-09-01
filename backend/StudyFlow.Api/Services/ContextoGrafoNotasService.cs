using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;

namespace StudyFlow.Api.Services;

public sealed class ContextoGrafoNotasService(IContextoGrafoNotasRepository contextoRepository) : IContextoGrafoNotasService
{
    public async Task<ContextoNotaDto?> ObterAsync(int notaId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var nota = await contextoRepository.ObterNotaDoUsuarioAsync(notaId, usuarioId, cancellationToken);
        if (nota is null)
            return null;

        var conexoes = await contextoRepository.ListarConexoesComNotasAsync(notaId, cancellationToken);

        var notasConectadas = conexoes
            .Select(conexao =>
            {
                var conectada = conexao.NotaOrigemId == notaId ? conexao.NotaDestino : conexao.NotaOrigem;
                return conectada is not null && conectada.UsuarioId == usuarioId
                    ? conectada.ToContextoConectadoDto(conexao.Rotulo)
                    : null;
            })
            .Where(x => x is not null)
            .Cast<ContextoNotaConectadaDto>()
            .DistinctBy(x => x.NotaId)
            .ToList();

        return nota.ToContextoDto(notasConectadas);
    }
}
