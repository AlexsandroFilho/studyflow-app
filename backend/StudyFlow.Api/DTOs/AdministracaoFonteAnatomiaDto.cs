using StudyFlow.Api.Domain.Enums;

namespace StudyFlow.Api.DTOs;

public sealed class CriarIngestaoFonteAnatomiaForm
{
    public IFormFile? Arquivo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Autor { get; set; }
    public string Versao { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public string? Subassunto { get; set; }
}

public sealed record CriarIngestaoFonteAnatomiaRequest(
    Stream Arquivo,
    string NomeArquivo,
    string Titulo,
    string? Autor,
    string Versao,
    string Assunto,
    string? Subassunto);

public sealed record IngestaoFonteAnatomiaResponseDto(
    Guid Id,
    string Titulo,
    string? Autor,
    string Versao,
    string Assunto,
    string? Subassunto,
    StatusIngestaoFonteAnatomia Status,
    string? MensagemErro,
    int QuantidadeChunks,
    Guid? FonteAnatomiaId,
    DateTime DataCriacao,
    DateTime? DataInicio,
    DateTime? DataConclusao);
