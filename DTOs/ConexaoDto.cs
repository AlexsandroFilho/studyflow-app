

using Microsoft.EntityFrameworkCore.Storage;

namespace StudyFlow.Api.DTOs
{
    public record CreateConexaoDto (
        int NotaOrigemId,
        int NotaDestinoId,
        string? Rotulo = null
    );

    public record CreateNotaConectadaDto(
        int NotaOrigemId,
        string Titulo,
        string Conteudo,
        int? TemaId = null,
        string? Rotulo = null
    );

    public record ConexaoResponseDto(
        int Id,
        int NotaOrigemId,
        int NotaDestinoId,
        string? Rotulo,
        DateTime DataCriacao
    );
}