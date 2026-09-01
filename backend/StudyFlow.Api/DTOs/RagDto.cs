using StudyFlow.Api.Domain.Enums;

namespace StudyFlow.Api.DTOs;

public sealed record ReferenciaAnatomiaDto(Guid FonteId, string Fonte, int Pagina, string? Secao, string? Assunto);
public sealed record ApontamentoRevisaoDto(string Tipo, string Trecho, string Explicacao, string? Sugestao);
public sealed record ResultadoRevisaoNotaDto(
    StatusRevisaoNota Status,
    string Resumo,
    IReadOnlyList<string> PontosCorretos,
    IReadOnlyList<ApontamentoRevisaoDto> Apontamentos,
    IReadOnlyList<ReferenciaAnatomiaDto> Referencias);
public sealed record RevisaoNotaResponseDto(Guid Id, int NotaId, ResultadoRevisaoNotaDto Resultado, string Modelo, DateTime DataCriacao);
public sealed record ContextoAnatomiaDto(Guid ChunkId, Guid FonteId, string Fonte, int Pagina, string? Secao, string? Assunto, string Texto, double Similaridade);
public sealed record ContextoNotaDto(int NotaId, string Titulo, string Conteudo, IReadOnlyList<ContextoNotaConectadaDto> Conexoes);
public sealed record ContextoNotaConectadaDto(int NotaId, string Titulo, string Conteudo, string? Rotulo);
public sealed record FonteIngestaoRequest(string CaminhoPdf, string Titulo, string? Autor, string Versao, string? Assunto, string? Subassunto);
public sealed record FonteIngestaoResultado(Guid FonteId, int QuantidadeChunks, bool Reindexada);
public sealed record ChunkAnatomiaDto(string Texto, int Pagina, string? Secao);
