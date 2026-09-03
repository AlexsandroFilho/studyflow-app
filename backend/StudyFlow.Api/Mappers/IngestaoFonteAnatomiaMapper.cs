using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers;

public static class IngestaoFonteAnatomiaMapper
{
    public static IngestaoFonteAnatomia ToEntity(this CriarIngestaoFonteAnatomiaRequest request, Guid usuarioId, string arquivoTemporarioChave) => new()
    {
        UsuarioId = usuarioId,
        Titulo = request.Titulo.Trim(),
        Autor = string.IsNullOrWhiteSpace(request.Autor) ? null : request.Autor.Trim(),
        Versao = request.Versao.Trim(),
        Assunto = request.Assunto.Trim(),
        Subassunto = string.IsNullOrWhiteSpace(request.Subassunto) ? null : request.Subassunto.Trim(),
        ArquivoTemporarioChave = arquivoTemporarioChave
    };

    public static IngestaoFonteAnatomiaResponseDto ToResponse(this IngestaoFonteAnatomia ingestao) => new(
        ingestao.Id,
        ingestao.Titulo,
        ingestao.Autor,
        ingestao.Versao,
        ingestao.Assunto,
        ingestao.Subassunto,
        ingestao.Status,
        ingestao.MensagemErro,
        ingestao.QuantidadeChunks,
        ingestao.FonteAnatomiaId,
        ingestao.DataCriacao,
        ingestao.DataInicio,
        ingestao.DataConclusao);
}
