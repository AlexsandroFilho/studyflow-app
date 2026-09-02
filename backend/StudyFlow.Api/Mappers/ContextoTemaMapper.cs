using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers;

public static class ContextoTemaMapper
{
    public static ContextoTemaNotaDto ToContextoTemaDto(this Nota nota) => new(nota.Id, nota.Titulo ?? string.Empty, nota.Conteudo ?? string.Empty);

    public static ContextoTemaConexaoDto ToContextoTemaDto(this ConexaoNota conexao) => new(
        conexao.Id,
        conexao.NotaOrigemId,
        conexao.NotaOrigem?.Titulo ?? throw new InvalidOperationException("Nota de origem não carregada."),
        conexao.NotaDestinoId,
        conexao.NotaDestino?.Titulo ?? throw new InvalidOperationException("Nota de destino não carregada."),
        conexao.Rotulo);

    public static ContextoTemaDto ToContextoTemaDto(this Tema tema, IReadOnlyList<ContextoTemaNotaDto> notas, IReadOnlyList<ContextoTemaConexaoDto> conexoes) =>
        new(tema.Id, tema.Nome ?? "Tema sem nome", tema.Descricao, notas, conexoes);
}
