namespace StudyFlow.Api.DTOs
{
    public record CreateNotaDto(
        string Titulo,
        string Conteudo,
        int TemaId,
        string? ResumoIa = null
    );

    public record UpdateNotaDto(
        string Titulo,
        string Conteudo,
        int TemaId,
        string? ResumoIa = null
    );

    public record NotaResponseDto(
        int Id,
        string Titulo,
        string Conteudo,
        string? ResumoIa,
        DateTime DataCriacao,
        int TemaId,
        string? NomeTema = null
    );
}